using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for RawViewer.xaml
    /// </summary>
    public sealed partial class RawViewer
    {
        private int _page = 1;

        public RawViewer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewPage(1, int.Parse(txtRecordsPerPage.Text, CultureInfo.CurrentCulture));
        }

        private void ViewPage(int page, int num)
        {
            DrawingsDataContext dc = DBCommon.NewDC;

            IQueryable<Drawing> res = (from d in dc.Drawings select d).Skip((page - 1)*num).Take(num);

            // Create the GridView
            var gv = new GridView();
            gv.AllowsColumnReorder = true;

            // Create the GridView Columns
            PropertyInfo[] pi = typeof (MPDrawing).GetProperties();


            foreach (PropertyInfo p in pi)
            {
                var gvc = new GridViewColumn();
                gvc.DisplayMemberBinding = new Binding(p.Name);
                gvc.Header = p.Name;
                gvc.Width = Double.NaN;
                gv.Columns.Add(gvc);
            }

            listView1.View = gv;
            listView1.ItemsSource = res;
        }

        private void btnNext_click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewPage(++_page, int.Parse(txtRecordsPerPage.Text, CultureInfo.CurrentCulture));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            lblPage.Content = "Page " + _page.ToString(CultureInfo.CurrentCulture);
        }

        private void btnPrevious_click(object sender, RoutedEventArgs e)
        {
            try
            {
                //dont want a negative page otherwise interesting things will happen
                if (--_page < 1)
                    ++_page;
                else
                    ViewPage(_page, int.Parse(txtRecordsPerPage.Text, CultureInfo.CurrentCulture));
                lblPage.Content = "Page " + _page.ToString(CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRefresh_click(object sender, RoutedEventArgs e)
        {
            ViewPage(_page, int.Parse(txtRecordsPerPage.Text, CultureInfo.CurrentCulture));
            lblPage.Content = "Page " + _page.ToString(CultureInfo.CurrentCulture);
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _page = 1;
                ViewPage(_page, int.Parse(txtRecordsPerPage.Text, CultureInfo.CurrentCulture));
                lblPage.Content = "Page " + _page.ToString(CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _page = (DBCommon.NewDC.Drawings.Count() - 1)/
                       int.Parse(txtRecordsPerPage.Text, CultureInfo.CurrentCulture) + 1;
                ViewPage(_page, int.Parse(txtRecordsPerPage.Text, CultureInfo.CurrentCulture));
                lblPage.Content = "Page " + _page.ToString(CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}