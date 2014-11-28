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
using MainPower.DrawingsDatabase.Gui.Views;
using MicroMvvm;

namespace MainPower.DrawingsDatabase.AutoCad
{
    /// <summary>
    /// The entry point for AutoCAD to execute commands
    /// </summary>
    public static class Commands
    {
        static Commands()
        {
            //because ACAD loads the plugin, any assemblies that are referenced by the GUI,
            //but not used by the plugin must be loaded manually

            //there must be a better way to do this??
            //perhaps though the config file or something.  meh
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string assPath = Path.GetDirectoryName(path);
            try
            {
                Assembly.LoadFrom(assPath + "\\" + "Xceed.Wpf.Toolkit.dll");
                Assembly.LoadFrom(assPath + "\\" + "MicroMvvm.dll");
                Assembly.LoadFrom(assPath + "\\" + "System.Windows.Interactivity.dll");
            }
            catch { }
        }

        [CommandMethod("ddbrevab")]
        public static void RevitAsBuilt()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.AddGenericRevision("AS BUILT");
        }

        [CommandMethod("ddbrevifc")]
        public static void RevitIssuedForConstruction()
        {
            DrawingCommands dc = new DrawingCommands();
            dc.AddGenericRevision("ISSUED FOR CONSTRUCTION");
        }
        
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

        [CommandMethod("ddblink")]
        public static void CreateAndEditDrawing()
        {
            DrawingCommands dc = new DrawingCommands();
            //create the drawings
            Drawing d = dc.CreateDefaultDrawingWithFileName();
            if (d == null)
                return;
            DrawingsDataContext ddc = DBCommon.NewDC;
            ddc.Drawings.InsertOnSubmit(d);
            //chuck it in the database
            ddc.SubmitChanges();
            //bring up the window to edit changes
            ViewDrawingWindow atv = new ViewDrawingWindow(d, null, true);            
            atv.ShowDialog();
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
            new AddTemplateDrawingView().Show();
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

       
    }

}
