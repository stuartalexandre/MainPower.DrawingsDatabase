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
using System.Linq.Dynamic;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for SubBrowser.xaml
    /// </summary>
    public sealed partial class SubBrowser : Window
    {
        public SubBrowser()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] subs = Properties.Settings.Default.Substations.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in subs)
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.Header = str;
                treeView1.Items.Add(tvi);
            }

            foreach (TreeViewItem tvi in treeView1.Items)
            {
                DrawingsDataContext dc = DBCommon.NewDC;

                var drawings = from d in dc.Drawings where d.ProjectTitle.Contains((string)tvi.Header) select d;

                foreach (MPDrawing d in drawings)
                {
                    tvi.Items.Add(d);
                }
                
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MPDrawing d = treeView1.SelectedItem as MPDrawing;

            if (d != null)
            {
                //Edit the drawing
                //ViewDrawingWindow win = new ViewDrawingWindow(d, null);
                //win.Show();
                //Open the drawing in cad
                try
                {
                    DBCommon.OpenAutoCadDrawing(d.FileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
