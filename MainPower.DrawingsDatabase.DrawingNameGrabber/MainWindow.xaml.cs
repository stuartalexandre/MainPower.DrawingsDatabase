using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using MainPower.DrawingsDatabase.DatabaseHelper;
using Microsoft.Win32;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;

namespace MainPower.DrawingsDatabase.DrawingNameGrabber
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ObservableCollection<DrawingInfo> _drawings = new ObservableCollection<DrawingInfo>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public IEnumerable<DrawingInfo> Drawings
        {
            get { return _drawings; }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = new OpenFileDialog();

                if ((bool) d.ShowDialog())
                {
                    var di = new DirectoryInfo(d.FileName);
                    txtPath.Text = di.Parent.FullName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _drawings.Clear();
                var di = new DirectoryInfo(txtPath.Text);

                ScanDirectory(di);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ScanDirectory(DirectoryInfo dir)
        {
            FileInfo[] fis = dir.GetFiles();
            foreach (FileInfo fi in fis)
            {
                if (fi.Extension.ToLower() == ".dwg" || fi.Extension.ToLower() == ".pdf" ||
                    fi.Extension.ToLower() == ".tif" || fi.Extension.ToLower() == ".xls" ||
                    fi.Extension.ToLower() == ".xlsx")
                {
                    var dri = new DrawingInfo();
                    dri.Name = fi.Name.Substring(0, fi.Name.Length - 4);
                    dri.Path = fi.FullName;
                    dri.Include = true;
                    _drawings.Add(dri);
                }
            }

            if (chkRecursive.IsChecked ?? false)
            {
                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    ScanDirectory(di);
                }
            }
        }

        private void btnUpdateDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DrawingsDataContext dc = DBCommon.NewDC;
                IEnumerable<DrawingInfo> selectedDrawings = (from d in _drawings where d.Include select d);

                foreach (DrawingInfo di in selectedDrawings)
                {
                    IQueryable<Drawing> dwgs =
                        (from MPDrawing d in dc.Drawings where d.Number.ToLower() == di.Name.ToLower() select d);
                    foreach (MPDrawing dwg in dwgs)
                    {
                        dwg.FileName = di.Path;
                    }
                }

                dc.SubmitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}