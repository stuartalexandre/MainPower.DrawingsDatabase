using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MainPower.DrawingsDatabase.Gui;

namespace MainPower.DrawingsDatabase.AutoCad
{
    /// <summary>
    /// The implementation of the drawing command functions
    /// </summary>
    public class DrawingCommands
    {
        private readonly List<string> _blockNames = new List<string> {"E-A3-L", "ENGINEERING - DYNAMIC TITLE BLOCK"};
        private readonly Database _db = HostApplicationServices.WorkingDatabase;
        private readonly Editor _ed = Application.DocumentManager.MdiActiveDocument.Editor;
        private readonly List<string> _sheetSizes = new List<string> {"A2P", "A3L", "A2L"};

        #region Command Functions

        public void DatabaseToDrawing()
        {
            Transaction tr = _db.TransactionManager.StartTransaction();
            try
            {
                IEnumerable<BlockReference> blocks = GetDrawingBlocks(tr, OpenMode.ForWrite);
                foreach (BlockReference block in blocks)
                {
                    DatabaseToBlock(block, tr);
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                _ed.WriteMessage(ex.Message);
            }
            finally
            {
                tr.Dispose();
            }
        }

        public void DatabaseToLayout()
        {
            Transaction tr = _db.TransactionManager.StartTransaction();
            try
            {
                IEnumerable<BlockReference> blocks = GetLayoutBlocks(tr, OpenMode.ForWrite);
                foreach (BlockReference block in blocks)
                {
                    DatabaseToBlock(block, tr);
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                _ed.WriteMessage(ex.Message);
            }
            finally
            {
                tr.Dispose();
            }
        }

        public void DatabaseToBlock()
        {
            Transaction tr = _db.TransactionManager.StartTransaction();
            try
            {
                IEnumerable<BlockReference> blocks = PromptForBlock(tr);
                foreach (BlockReference blkRef in blocks)
                {
                    DatabaseToBlock(blkRef, tr);
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                _ed.WriteMessage(("Exception in DatabaseToBlock(): " + ex.Message + ex.StackTrace));
            }
            finally
            {
                tr.Dispose();
            }
        }

        public void DrawingToDatabase()
        {
            Transaction tr = _db.TransactionManager.StartTransaction();
            try
            {
                IEnumerable<BlockReference> blocks = GetDrawingBlocks(tr, OpenMode.ForRead);
                foreach (BlockReference block in blocks)
                {
                    BlockToDatabase(block, tr);
                }

                tr.Commit();
            }
            catch (Exception ex)
            {
                _ed.WriteMessage(ex.Message);
            }
            finally
            {
                tr.Dispose();
            }
        }

        public void LayoutToDatabase()
        {
            Transaction tr = _db.TransactionManager.StartTransaction();
            try
            {
                IEnumerable<BlockReference> blocks = GetLayoutBlocks(tr, OpenMode.ForRead);
                foreach (BlockReference block in blocks)
                {
                    BlockToDatabase(block, tr);
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                _ed.WriteMessage(ex.Message);
            }
            finally
            {
                tr.Dispose();
            }
        }

        public void BlockToDatabase()
        {
            Transaction tr = _db.TransactionManager.StartTransaction();
            try
            {
                IEnumerable<BlockReference> blocks = PromptForBlock(tr);
                foreach (BlockReference blkRef in blocks)
                {
                    BlockToDatabase(blkRef, tr);
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                _ed.WriteMessage(("Exception: " + ex.Message));
            }
            finally
            {
                tr.Dispose();
            }
        }

        public void DumpBlockAttributes()
        {
            Transaction tr = _db.TransactionManager.StartTransaction();
            // Start the transaction
            try
            {
                IEnumerable<BlockReference> blocks = PromptForBlock(tr);

                foreach (BlockReference blkRef in blocks)
                {
                    AttributeCollection attCol = blkRef.AttributeCollection;
                    foreach (ObjectId attId in attCol)
                    {
                        var attRef = (AttributeReference) tr.GetObject(attId, OpenMode.ForWrite);
                        _ed.WriteMessage(attRef.Tag + ": " + attRef.TextString + "\n");
                    }
                    _ed.WriteMessage("Name: " + blkRef.Name + "\n");
                    foreach (DynamicBlockReferenceProperty p in blkRef.DynamicBlockReferencePropertyCollection)
                    {
                        _ed.WriteMessage(p.PropertyName + ": " + p.Value + "\n");
                    }
                    _ed.WriteMessage("IsDynamic: " + blkRef.IsDynamicBlock.ToString(CultureInfo.InvariantCulture) + "\n");
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                _ed.WriteMessage(("Exception: " + ex.Message));
            }
            finally
            {
                tr.Dispose();
            }
        }

        public void NeedAnotherRevision()
        {
            Transaction tr = _db.TransactionManager.StartTransaction();
            // Start the transaction

            // Build a filter list so that only block references are selected
            var filList = new[] {new TypedValue((int) DxfCode.Start, "INSERT")};
            var filter = new SelectionFilter(filList);
            var opts = new PromptSelectionOptions {MessageForAdding = "Select block references: "};

            PromptSelectionResult res = _ed.GetSelection(opts, filter);

            // Do nothing if selection is unsuccessful
            if (res.Status != PromptStatus.OK)
                return;

            SelectionSet selSet = res.Value;
            ObjectId[] idArray = selSet.GetObjectIds();
            foreach (ObjectId blkId in idArray)
            {
                var blkRef = (BlockReference) tr.GetObject(blkId, OpenMode.ForWrite);
                //TODO: block validation here
                AttributeCollection attCol = blkRef.AttributeCollection;
                Dictionary<string, AttributeReference> attribs =
                    (from ObjectId attId in attCol
                     select (AttributeReference) tr.GetObject(attId, OpenMode.ForWrite)).ToDictionary(
                         attRef => attRef.Tag);

                attribs["REV-A"].TextString = attribs["REV-B"].TextString;
                attribs["BY-A"].TextString = attribs["BY-B"].TextString;
                attribs["DESCRIPTION-A"].TextString = attribs["DESCRIPTION-B"].TextString;
                attribs["DATE-A"].TextString = attribs["DATE-B"].TextString;
                attribs["APPRV-BY-A"].TextString = attribs["APPRV-BY-B"].TextString;

                attribs["REV-B"].TextString = attribs["REV-C"].TextString;
                attribs["BY-B"].TextString = attribs["BY-C"].TextString;
                attribs["DESCRIPTION-B"].TextString = attribs["DESCRIPTION-C"].TextString;
                attribs["DATE-B"].TextString = attribs["DATE-C"].TextString;
                attribs["APPRV-BY-B"].TextString = attribs["APPRV-BY-C"].TextString;

                attribs["REV-C"].TextString = attribs["REV-D"].TextString;
                attribs["BY-C"].TextString = attribs["BY-D"].TextString;
                attribs["DESCRIPTION-C"].TextString = attribs["DESCRIPTION-D"].TextString;
                attribs["DATE-C"].TextString = attribs["DATE-D"].TextString;
                attribs["APPRV-BY-C"].TextString = attribs["APPRV-BY-D"].TextString;

                attribs["REV-D"].TextString = attribs["REV-E"].TextString;
                attribs["BY-D"].TextString = attribs["BY-E"].TextString;
                attribs["DESCRIPTION-D"].TextString = attribs["DESCRIPTION-E"].TextString;
                attribs["DATE-D"].TextString = attribs["DATE-E"].TextString;
                attribs["APPRV-BY-D"].TextString = attribs["APPRV-BY-E"].TextString;

                attribs["REV-E"].TextString = attribs["REV-F"].TextString;
                attribs["BY-E"].TextString = attribs["BY-F"].TextString;
                attribs["DESCRIPTION-E"].TextString = attribs["DESCRIPTION-F"].TextString;
                attribs["DATE-E"].TextString = attribs["DATE-F"].TextString;
                attribs["APPRV-BY-E"].TextString = attribs["APPRV-BY-F"].TextString;

                attribs["REV-F"].TextString = attribs["REV-G"].TextString;
                attribs["BY-F"].TextString = attribs["BY-G"].TextString;
                attribs["DESCRIPTION-F"].TextString = attribs["DESCRIPTION-G"].TextString;
                attribs["DATE-F"].TextString = attribs["DATE-G"].TextString;
                attribs["APPRV-BY-F"].TextString = attribs["APPRV-BY-G"].TextString;

                attribs["REV-G"].TextString = "";
                attribs["BY-G"].TextString = "";
                attribs["DESCRIPTION-G"].TextString = "";
                attribs["DATE-G"].TextString = "";
                attribs["APPRV-BY-G"].TextString = "";
            }
            tr.Commit();
        }

        /// <summary>
        /// Fills a title blocks drawing number with the next available number, then saves it to the database
        /// (so a subsequent call to this function will give the next number, not the same one)
        /// </summary>
        public void AutoNumberBlockThenSave()
        {
            Transaction tr = _db.TransactionManager.StartTransaction();
            try
            {
                IEnumerable<BlockReference> blocks = PromptForBlock(tr);

                foreach (BlockReference blkRef in blocks)
                {
                    Dictionary<string, AttributeReference> attribs = GetBlockAttributes(blkRef, tr);

                    attribs["DRAWING-NUMBER"].TextString = DBCommon.GetNextDrawingNumber();
                    if (!BlockToDatabase(blkRef, tr))
                    {
                        _ed.WriteMessage("Could not save drawing to database, returning drawing number to the pool \n");
                        attribs["DRAWING-NUMBER"].TextString = "";
                    }
                }

                tr.Commit();
            }
            catch (Exception ex)
            {
                _ed.WriteMessage(ex.Message + "\n");
            }
            finally
            {
                tr.Dispose();
            }
        }

        /// <summary>
        /// Show the ViewDrawingWindow for the current drawing
        /// </summary>
        public void ShowViewDrawingWindow(bool enableEditByDefault)
        {
            Transaction tr = null;
            try
            {
                DrawingsDataContext ddc = DBCommon.NewDC;
                tr = _db.TransactionManager.StartTransaction();

                IEnumerable<BlockReference> blocks = GetLayoutBlocks(tr, OpenMode.ForRead);

                Drawing d = null;

                foreach (BlockReference block in blocks)
                {
                    if (d != null)
                        return;
                    d = GetDrawingFromBlock(ddc, block, tr);
                }

                if (d == null)
                    return;

                var w = new ViewDrawingWindow(d, null, enableEditByDefault);
                w.ShowDialog();
            }
            catch (Exception ex)
            {
                _ed.WriteMessage(ex.ToString());
            }
            finally
            {
                if (tr != null) tr.Dispose();
            }
        }

        public Drawing CreateDefaultDrawingWithFileName()
        {
            try
            {
                Drawing d = DBCommon.CreateDefaultDrawing();
                d.FileName = _db.Filename;
                return d;
            }
            catch { return null; }
        }

        public void PrintVersion()
        {
            _ed.WriteMessage("MainPower Drawings Database Version: " + GetType().Assembly.GetName().Version);
        }

        #endregion

        #region Helper Functions

        private bool BlockToDatabase(BlockReference block, Transaction tr)
        {
            try
            {
                Dictionary<string, AttributeReference> attribs = GetBlockAttributes(block, tr);

                if (String.IsNullOrEmpty(attribs["DRAWING-NUMBER"].TextString))
                {
                    PromptResult pr1 = _ed.GetString("Drawing Number:");
                    attribs["DRAWING-NUMBER"].TextString = pr1.StringResult;
                }
                if (String.IsNullOrEmpty(attribs["SHEET"].TextString))
                {
                    PromptResult pr1 = _ed.GetString("Sheet Number:");
                    attribs["SHEET"].TextString = pr1.StringResult;
                }
                if (String.IsNullOrEmpty(attribs["DRAWING-NUMBER"].TextString) ||
                    String.IsNullOrEmpty(attribs["SHEET"].TextString))
                {
                    _ed.WriteMessage("No drawing/sheet number forthcoming, returning.\n");
                    return false;
                }
                DrawingsDataContext dc = DBCommon.NewDC;
                Drawing drawing = GetDrawing(dc, attribs["DRAWING-NUMBER"].TextString, attribs["SHEET"].TextString);

                if (drawing == null)
                {
                    drawing = new Drawing();
                    dc.Drawings.InsertOnSubmit(drawing);
                }

                foreach (var kvp in attribs)
                {
                    if (DrawingHelper.AttributeExists(kvp.Key)) //only save the desired attributes
                        drawing.SetAttribute(kvp.Key, kvp.Value.TextString);
                }

                drawing.SheetSize = GetSheetSizeFromBlock(block, tr);

                //TODO: check this
                if (_db.Filename.EndsWith("sv$", StringComparison.OrdinalIgnoreCase))
                {
                    _ed.WriteMessage("File has not been saved since autosave, filename not put into the database.\n");
                }
                else
                {
                    drawing.FileName = _db.Filename;

                    //drawing.Filename = _db.OriginalFileName;
                }
                //If we're putting this in via the CAD, then it must be electronic
                drawing.Electronic = true;

                dc.SubmitChanges();

                if (drawing.Category == DrawingCategory.Undefined)
                    _ed.WriteMessage("WARNING:  Drawing Category is undefined!\n");
                if (drawing.Status == DrawingStatus.Undefined)
                    _ed.WriteMessage("WARNING:  Drawing Status is undefined!\n");
                _ed.WriteMessage("Data successfully written to the database from block " + block.Name + "\n");
                return true;
            }
            catch (Exception ex)
            {
                _ed.WriteMessage(ex.Message);
                return false;
            }
        }

        private void DatabaseToBlock(BlockReference block, Transaction tr)
        {
            try
            {
                Dictionary<string, AttributeReference> attribs = GetBlockAttributes(block, tr);
                DrawingsDataContext dc = DBCommon.NewDC;

                Drawing drawing = GetDrawing(dc, attribs["DRAWING-NUMBER"].TextString, attribs["SHEET"].TextString);

                if (drawing == null)
                {
                    _ed.WriteMessage("Drawing was not found, check Drawing Number AND Sheet Number are correct \n");
                    return;
                }

                foreach (var kvp in attribs)
                {
                    if (DrawingHelper.AttributeExists(kvp.Key)) //only save the desired attributes
                        kvp.Value.TextString = drawing.GetAttribute(kvp.Key);
                }

                SetBlockSheetSize(block, drawing.SheetSize, tr);

                _ed.WriteMessage("Data successfully fetched from the database and put in block " + block.Name + "\n");
            }
            catch (Exception ex)
            {
                _ed.WriteMessage("Exception at DatabaseToBlock(block, tr): " + ex.Message + ex.StackTrace + "\n");
            }
        }

        /// <summary>
        /// Get the blocks in a layout
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private IEnumerable<BlockReference> GetLayoutBlocks(Transaction tr, OpenMode mode)
        {
            var blocks = new List<BlockReference>();
            //Get the drawings layout dictionary
            ObjectId curLayoutID = LayoutManager.Current.GetLayoutId(LayoutManager.Current.CurrentLayout);
            var layout = curLayoutID.GetObject(OpenMode.ForRead) as Layout;
            if (layout != null)
            {
                if (tr != null)
                {
                    var layoutRecord = tr.GetObject(layout.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;
                    //Get the enumerator
                    if (layoutRecord != null)
                    {
                        BlockTableRecordEnumerator recordEnumerator = layoutRecord.GetEnumerator();
                        //Loop through all blocks in the block table
                        while (recordEnumerator.MoveNext())
                        {
                            var blcokEnt = tr.GetObject(recordEnumerator.Current, mode) as Entity;
                            BlockReference oBr;
                            if ((oBr = blcokEnt as BlockReference) != null)
                            {
                                blocks.Add(oBr);
                            }
                        }
                    }
                }
            }

            return blocks.Where(b => ValidateBlock(b, tr)).ToList();
        }

        /// <summary>
        /// Gets all the blocks in a drawing
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private IEnumerable<BlockReference> GetDrawingBlocks(Transaction tr, OpenMode mode)
        {
            var blocks = new List<BlockReference>();
            //Get the drawings layout dictionary
            var layoutDict = tr.GetObject(_db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
            if (layoutDict != null)
                foreach (DictionaryEntry id in layoutDict)
                {
                    //Get the layout object
                    var layout = tr.GetObject((ObjectId) id.Value, OpenMode.ForRead) as Layout;
                    if (layout == null) continue;
                    var layoutRecord = tr.GetObject(layout.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;
                    //Get the enumerator
                    if (layoutRecord != null)
                    {
                        BlockTableRecordEnumerator recordEnumerator = layoutRecord.GetEnumerator();
                        //Loop through all blocks in the block table

                        while (recordEnumerator.MoveNext())
                        {
                            var blcokEnt = tr.GetObject(recordEnumerator.Current, mode) as Entity;
                            BlockReference oBr;
                            if ((oBr = blcokEnt as BlockReference) != null)
                            {
                                blocks.Add(oBr);
                            }
                        }
                    }
                }

            return blocks.Where(b => ValidateBlock(b, tr)).ToList();
        }

        private bool ValidateBlock(BlockReference blkRef, Transaction tr)
        {
            if (_blockNames.Contains(blkRef.Name))
                return true;
            if (
                _blockNames.Contains(
                    ((BlockTableRecord) (tr.GetObject(blkRef.DynamicBlockTableRecord, OpenMode.ForRead))).Name))
                return true;

            return false;
        }

        private string GetSheetSizeFromBlock(BlockReference blkRef, Transaction tr)
        {
            try
            {
                //TODO: Return other cases here as well
                switch (blkRef.GetBlockName(tr))
                {
                    case "E-A3-L":
                        return "A3L";
                    case "ENGINEERING - DYNAMIC TITLE BLOCK":
                        return (from DynamicBlockReferenceProperty d in blkRef.DynamicBlockReferencePropertyCollection
                                where d.PropertyName == "Visibility1"
                                select d).First().Value.ToString();
                    default:
                        return "";
                }
            }
            catch (Exception ex)
            {
                _ed.WriteMessage("Unexpected error in GetSheetSizeFromBlock" + "\n" + ex.Message + "\n");
                return "";
            }
        }

        private void SetBlockSheetSize(BlockReference blkRef, string sheetSize, Transaction tr)
        {
            try
            {
                //TODO: Return other cases here as well
                if (!_sheetSizes.Contains(sheetSize))
                    return;
                switch (blkRef.GetBlockName(tr))
                {
                    case "ENGINEERING - DYNAMIC TITLE BLOCK":
                        DynamicBlockReferenceProperty p =
                            (from DynamicBlockReferenceProperty d in blkRef.DynamicBlockReferencePropertyCollection
                             where d.PropertyName == "Visibility1"
                             select d).First();
                        p.Value = sheetSize;
                        break;
                }
            }
            catch (Exception ex)
            {
                _ed.WriteMessage("Unexpected error in SetBlockSheetSize()" + "\n" + ex.Message + "\n");
            }
        }

        private static Drawing GetDrawingFromBlock(DrawingsDataContext dc, BlockReference blkRef, Transaction tr)
        {
            Dictionary<string, AttributeReference> attribs = GetBlockAttributes(blkRef, tr);
            return
                dc.Drawings.FirstOrDefault(
                    d => d.Number == attribs["DRAWING-NUMBER"].TextString && d.Sheet == attribs["SHEET"].TextString);
        }

        /// <summary>
        /// Prompts the user for a selection of blocks, returns a list of block references
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        private IEnumerable<BlockReference> PromptForBlock(Transaction tr)
        {
            var blocks = new List<BlockReference>();
            var filList = new[] {new TypedValue((int) DxfCode.Start, "INSERT")};
            var filter = new SelectionFilter(filList);
            var opts = new PromptSelectionOptions {MessageForAdding = "Select block references: "};

            PromptSelectionResult res = _ed.GetSelection(opts, filter);

            if (res.Status == PromptStatus.OK)
            {
                SelectionSet selSet = res.Value;
                ObjectId[] idArray = selSet.GetObjectIds();
                blocks.AddRange(
                    idArray.Select(blkId => (BlockReference) tr.GetObject(blkId, OpenMode.ForWrite)).Where(
                        blkRef => ValidateBlock(blkRef, tr)));
            }
            return blocks;
        }

        private static Dictionary<string, AttributeReference> GetBlockAttributes(BlockReference blkRef, Transaction tr)
        {
            AttributeCollection attCol = blkRef.AttributeCollection;

            return
                (from ObjectId attId in attCol select (AttributeReference) tr.GetObject(attId, OpenMode.ForWrite)).
                    ToDictionary(attRef => attRef.Tag);
        }

        private static Drawing GetDrawing(DrawingsDataContext dc, string drawingNumber, string sheetNumber)
        {
            IQueryable<Drawing> drawings =
                (from d in dc.Drawings where d.Number == drawingNumber && d.Sheet == sheetNumber select d);
            return !drawings.Any() ? null : drawings.First();
        }

        #endregion

       
    }

    /// <summary>
    /// Various Extension Methods
    /// </summary>
    public static class DrawingExtensions
    {
        /// <summary>
        /// Gets the block name, if the block is anonymous, then get the original dynamic block name.
        /// </summary>
        /// <param name="blockRef"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static string GetBlockName(this BlockReference blockRef, Transaction transaction)
        {
            return blockRef.IsDynamicBlock
                       ? ((BlockTableRecord) (transaction.GetObject(blockRef.DynamicBlockTableRecord, OpenMode.ForRead)))
                             .Name
                       : blockRef.Name;
        }
    }
}