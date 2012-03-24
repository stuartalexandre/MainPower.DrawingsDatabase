using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MainPower.DrawingsDatabase.Gui.Properties;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for SubBrowser.xaml
    /// </summary>
    public sealed partial class SubBrowser
    {
        public SubBrowser()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] subs = Settings.Default.Substations.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in subs)
            {
                var tvi = new TreeViewItem();
                tvi.Header = str;
                treeView1.Items.Add(tvi);
            }

            foreach (TreeViewItem tvi in treeView1.Items)
            {
                DrawingsDataContext dc = DBCommon.NewDC;

                IQueryable<Drawing> drawings = from d in dc.Drawings
                                               where d.ProjectTitle.Contains((string) tvi.Header)
                                               select d;

                foreach (MPDrawing d in drawings)
                {
                    tvi.Items.Add(d);
                }
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var d = treeView1.SelectedItem as MPDrawing;

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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}