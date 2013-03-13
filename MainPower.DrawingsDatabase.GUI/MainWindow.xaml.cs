using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;
using System.Globalization;
using System.Reflection;
using System.IO;
using MainPower.DrawingsDatabase.Gui.Properties;
using MainPower.DrawingsDatabase.Gui.Views;
using MainPower.DrawingsDatabase.Gui.ViewModels;
using System.Xml.Serialization;
using MainPower.DrawingsDatabase.Gui.Models;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for PageWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
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
            TabItem tabItem = args.Source as TabItem;
            if (tabItem != null)
            {
                TabControl tabControl = tabItem.Parent as TabControl;
                if (tabControl != null)
                    tabControl.Items.Remove(tabItem);
            }
        }

        public SearchView ActiveSearch
        {
            get
            {
                Frame f = (tabControl1.SelectedContent as Grid).Children[0] as Frame;
                return f.Content as SearchView;
                
            }
        }


        private void mnuHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "\\" + "DrawingsDatabase.chm");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void mnuAdd_Click(object sender, RoutedEventArgs e)
        {
            //new AddDrawingWindow().Show();
            try
            {
                new AddTemplateDrawingView().Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Add a new search tab to the tab control
        /// </summary>
        private void AddSearchTab()
        {
            CloseableTabItem cti = new CloseableTabItem();
            cti.Header = "New Search " + _searchNumber.ToString(CultureInfo.CurrentCulture);
            _searchNumber++;
            Grid g = new Grid();
            Frame f = new Frame();
            SearchView sv = new SearchView();
            SearchViewModel svm = sv.DataContext as SearchViewModel;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.SavedSearch))
            {
                MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Settings.Default.SavedSearch));
                XmlSerializer xml = new XmlSerializer(typeof(DrawingSearchModel));
                DrawingSearchModel dsm = xml.Deserialize(ms) as DrawingSearchModel;
                svm.Model = dsm;
            }

            f.Navigate(sv);
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

        private void mnuNextDwgNo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The next available drawing number is: " + DBCommon.GetNextDrawingNumber());
        }

        private void mnuGUISettings_Click(object sender, RoutedEventArgs e)
        {
            new GuiSettings().ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //event handler to tell the parent tab control to close the tab
            this.AddHandler(CloseableTabItem.CloseTabEvent, new RoutedEventHandler(this.CloseTab));
            AddSearchTab();
        }

        private void mnuPrintResults_Click(object sender, RoutedEventArgs e)
        {
            //Frame f = (tabControl1.SelectedContent as Grid).Children[0] as Frame;
            //SearchPage sp = f.Content as SearchPage;
            //new PrintPreview(sp.CreateXps, Settings.Default.PageSize, Settings.Default.PageOrientation).ShowDialog();
        }

        private void mnuInstallPlugin2010_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DBCommon.InstallPlugin(AcadVersion.ACAD2010);
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
                DBCommon.InstallPlugin(AcadVersion.ACAD2012);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mnuRemovePlugin2012_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DBCommon.RemovePlugin(AcadVersion.ACAD2012);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mnuRemovePlugin2010_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DBCommon.RemovePlugin(AcadVersion.ACAD2010);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NewTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddSearchTab();
        }

        private void mnuSaveSearchSettings_Click(object sender, RoutedEventArgs e)
        {
            //TODO: trap and log errors here...
            if (ActiveSearch == null)
                return;
            ActiveSearch.PersistColumns();
            SearchViewModel svm = ActiveSearch.DataContext as SearchViewModel;

            MemoryStream ms = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(typeof(DrawingSearchModel));
            xml.Serialize(ms, svm.Model);
            Properties.Settings.Default.SavedSearch = Encoding.ASCII.GetString(ms.GetBuffer());
            Properties.Settings.Default.Save();
        }

        private void mnuExportSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveSearch == null)
                return;
            var svm = ActiveSearch.DataContext as SearchViewModel;
            svm.ExportSearch.Execute(null);
        }
        
    }
}
