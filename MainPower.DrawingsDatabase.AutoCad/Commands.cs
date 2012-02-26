using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MainPower.DrawingsDatabase.Gui;
using System.Data.SqlClient;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices;
using System.Collections;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Customization;
using System.Collections.Specialized;
using System.IO;

namespace MainPower.DrawingsDatabase.AutoCad
{
    /// <summary>
    /// The entry point for AutoCAD to execute commands
    /// </summary>
    public static class Commands
    {
        [CommandMethod("ddbmorerev")]
        public static void NeedAnotherRevision()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.NeedAnotherRevision();
        }

        [CommandMethod("ddbputd")]
        public static void DrawingToDatabase()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.DrawingToDatabase();
        }

        [CommandMethod("ddbputl")]
        public static void LayoutToDatabase()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.LayoutToDatabase();
        }

        [CommandMethod("ddbput")]
        public static void BlockToDatabase()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.BlockToDatabase();
        }

        [CommandMethod("ddbgetd")]
        public static void DatabaseToDrawing()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.DatabaseToDrawing();
        }

        [CommandMethod("ddbgetl")]
        public static void DatabaseToLayout()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.DatabaseToLayout();
        }

        [CommandMethod("ddbget")]
        public static void DatabaseToBlock()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.DatabaseToBlock();
        }

     
        [CommandMethod("ddbnum")]
        public static void AutoNumberBlockThenSave()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.AutoNumberBlockThenSave();
        }

        [CommandMethod("ddbversion")]
        public static void PrintVersion()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.PrintVersion();
        }

        //
        // Gui commands
        //
        [CommandMethod("ddbsettings")]
        public static void ShowSettingsWindow()
        {
            SettingsWindow win = new SettingsWindow();
            win.ShowDialog();
        }

        [CommandMethod("ddbsearch")]
        public static void ShowSearchWindow()
        {
            try
            {
                MainWindow mw = new MainWindow();
                mw.Show();
            }
            catch (System.Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.Message + ex.StackTrace);
            }
        }

        [CommandMethod("ddbadd")]
        public static void ShowAddDrawingWindow()
        {
            new AddDrawingWindow().Show();
        }

        [CommandMethod("ddbabout")]
        public static void ShowAboutWindow()
        {
            AboutWindow mw = new AboutWindow();
            mw.ShowDialog();
        }

        [CommandMethod("ddbedit")]
        public static void ShowViewDrawingWindowThenUpdate()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.ShowViewDrawingWindow(true);
            dc.DatabaseToLayout();
        }

        /*
        [CommandMethod("ddbcreatecui")]
        public void BuildMenuCUI()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            
            PromptFileNameResult res = ed.GetFileNameForSave("cuix file name");

            if (res.Status == PromptStatus.OK)
            {
                DirectoryInfo di = Directory.GetParent(res.StringResult);
                ed.WriteMessage(di.FullName);

                string myCuiFile = di.FullName + "\\ddb.cuix";            //"C:\\Users\\hsc\\Desktop\\ddb.cuix";
                string myCuiFileToSend = myCuiFile.Replace("\\", "/");  //"C:/Users/hsc/Desktop/ddb.cuix";
                string myCuiSectionName = "MPRDDB";


                //string mainCui = Application.GetSystemVariable("MENUNAME") + ".cui";
                CustomizationSection cs = new CustomizationSection();
                PartialCuiFileCollection pcfc = cs.PartialCuiFiles;

                if (pcfc.Contains(myCuiFile))
                {
                    ed.WriteMessage("\nCustomization file \"" + myCuiFile + "\" already loaded.");
                }
                else
                {
                    if (System.IO.File.Exists(myCuiFile))
                    {
                       ed.WriteMessage("\nCustomization file \"" + myCuiFile + "\" exists");
                        //LoadMyCui(myCuiFileToSend);
                    }
                    else
                    {
                        ed.WriteMessage("\nCustomization file \"" + myCuiFile + "\" does not exist - building it.");

                        // Create a customization section for our partial menu
                        CustomizationSection pcs = new CustomizationSection();
                        pcs.MenuGroupName = myCuiSectionName;

                        // Let's add a menu group, with two commands
                        MacroGroup mg = new MacroGroup(myCuiSectionName, pcs.MenuGroup);
                        MenuMacro mm1 = new MenuMacro(mg, "Put title block data into the drawings database", "^C^Cddbput", "ddbcmdput");
                        MenuMacro mm2 = new MenuMacro(mg, "Get title block data from the drawings database", "^C^Cddbget", "ddbcmdget");

                        Properties.Resources.get.Save(di.FullName + "\\getImage.bmp");
                        Properties.Resources.put.Save(di.FullName + "\\putImage.bmp");

                        mm1.macro.LargeImage = mm1.macro.SmallImage = di.FullName + "\\putImage.bmp";
                        mm2.macro.LargeImage = mm2.macro.SmallImage = di.FullName + "\\getImage.bmp";


                        // Finally we save the file and load it
                        pcs.SaveAs(myCuiFile);
                        //LoadMyCui(myCuiFileToSend);
                    }
                }
            }
        }

        private void LoadMyCui(string cuiFile)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;

            object oldCmdEcho = Application.GetSystemVariable("CMDECHO");
            object oldFileDia = Application.GetSystemVariable("FILEDIA");

            Application.SetSystemVariable("CMDECHO", 0);
            Application.SetSystemVariable("FILEDIA", 0);

            doc.SendStringToExecute("_.cuiload " + cuiFile + " ", false, false, false);
            doc.SendStringToExecute("(setvar \"FILEDIA\" " + oldFileDia.ToString() + ")(princ) ", false, false, false);
            doc.SendStringToExecute("(setvar \"CMDECHO\" " + oldCmdEcho.ToString() + ")(princ) ", false, false, false);
        }
        */
    }

}
