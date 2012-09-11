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
using HC.Utils.Controls;
using System.Globalization;
using System.Reflection;
using System.IO;
using HC.Utils;
using MainPower.DrawingsDatabase.Gui.Properties;
using MainPower.DrawingsDatabase.Gui.Views;

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
            new AddTemplateDrawingView().Show();
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
            f.Navigate(new SearchView());
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
                DBCommon.RemovePlugin(AcadVersion.ACAD2012);
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
        
    }
}
