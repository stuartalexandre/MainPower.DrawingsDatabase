using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HC.Utils;
using HC.Utils.Controls;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MainPower.DrawingsDatabase.Gui.Properties;
using Microsoft.Win32;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for PageWindow.xaml
    /// </summary>
    public sealed partial class MainWindow
    {
        private int _searchNumber = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Close tab event handler. Closes the tab when the 'x' in the corner of the tab is clicked
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void CloseTab(object source, RoutedEventArgs args)
        {
            var tabItem = args.Source as TabItem;
            if (tabItem != null)
            {
                var tabControl = tabItem.Parent as TabControl;
                if (tabControl != null)
                    tabControl.Items.Remove(tabItem);
            }
        }

        private void mnuHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "\\" +
                              "DrawingsDatabase.chm");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void mnuAdd_Click(object sender, RoutedEventArgs e)
        {
            new AddDrawingWindow().Show();
        }

        /// <summary>
        /// Add a new search tab to the tab control
        /// </summary>
        private void AddSearchTab()
        {
            var cti = new CloseableTabItem();
            cti.Header = "New Search " + _searchNumber.ToString(CultureInfo.CurrentCulture);
            _searchNumber++;
            var g = new Grid();
            var f = new Frame();
            f.Navigate(new SearchPage());
            g.Children.Add(f);

            cti.Content = g;
            tabControl1.Items.Add(cti);
            tabControl1.SelectedItem = cti;
        }

        private void mnuSearch_Click(object sender, RoutedEventArgs e)
        {
            AddSearchTab();
        }

        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        private void mnuEditDbConnection_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private void mnuDbViewer_Click(object sender, RoutedEventArgs e)
        {
            new RawViewer().Show();
        }

        private void mnuSubBrowser_Click(object sender, RoutedEventArgs e)
        {
            new SubBrowser().Show();
        }

        private void mnuInstallPlugin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                const string rootPath = "HKEY_CURRENT_USER\\Software\\Autodesk\\AutoCAD\\R18.0\\ACAD-8001:409\\Applications\\DrawingsDB";

                //DESCRIPTION
                const string description = "Drawings database helper";
                //LOADER
                string loader = AppDomain.CurrentDomain.BaseDirectory + "AutoCADPlugin.dll";
                //LOADCTRLS
                const int loadCtrls = 2;
                //MANAGED
                const int managed = 1;


                Registry.SetValue(rootPath, "DESCRIPTION", description);
                Registry.SetValue(rootPath, "LOADER", loader);
                Registry.SetValue(rootPath, "LOADCTRLS", loadCtrls, RegistryValueKind.DWord);
                Registry.SetValue(rootPath, "MANAGED", managed, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mnuNextDwgNo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The next available drawing number is: " + DBCommon.GetNextDrawingNumber());
        }

        private void mnuRemovePlugin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //TODO: test this remove plugin code
                Registry.CurrentUser.DeleteSubKey(
                    "Software\\Autodesk\\AutoCAD\\R18.0\\ACAD-8001:409\\Applications\\DrawingsDB");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mnuGUISettings_Click(object sender, RoutedEventArgs e)
        {
            new GuiSettings().ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //event handler to tell the parent tab control to close the tab
            AddHandler(CloseableTabItem.CloseTabEvent, new RoutedEventHandler(CloseTab));
            AddSearchTab();
        }

        private void mnuPrintResults_Click(object sender, RoutedEventArgs e)
        {
            var f = (tabControl1.SelectedContent as Grid).Children[0] as Frame;
            var sp = f.Content as SearchPage;
            new PrintPreview(sp.CreateXps, Settings.Default.PageSize, Settings.Default.PageOrientation).ShowDialog();
        }

        private void mnuRemovePlugin2012_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //TODO: test this remove plugin code
                Registry.CurrentUser.DeleteSubKey(
                    "Software\\Autodesk\\AutoCAD\\R18.2\\ACAD-A001:409\\Applications\\DrawingsDB");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mnuInstallPlugin2012_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                const string rootPath = "HKEY_CURRENT_USER\\Software\\Autodesk\\AutoCAD\\R18.2\\ACAD-A001:409\\Applications\\DrawingsDB";

                //DESCRIPTION
                const string description = "Drawings database helper";
                //LOADER
                string loader = AppDomain.CurrentDomain.BaseDirectory + "AutoCADPlugin.dll";
                //LOADCTRLS
                const int loadCtrls = 2;
                //MANAGED
                const int managed = 1;


                Registry.SetValue(rootPath, "DESCRIPTION", description);
                Registry.SetValue(rootPath, "LOADER", loader);
                Registry.SetValue(rootPath, "LOADCTRLS", loadCtrls, RegistryValueKind.DWord);
                Registry.SetValue(rootPath, "MANAGED", managed, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mnuColumnsLayoutSave_Click(object sender, RoutedEventArgs e)
        {
            var cti = tabControl1.SelectedItem as CloseableTabItem;
            var g = cti.Content as Grid;
            var f = g.Children[0] as Frame;
            var sp = f.Content as SearchPage;

            sp.PersistColumnLayout();
        }

        private void NewTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddSearchTab();
        }
    }
}