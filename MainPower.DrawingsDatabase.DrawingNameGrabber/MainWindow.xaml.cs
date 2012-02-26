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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;

namespace MainPower.DrawingsDatabase.DrawingNameGrabber
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private ObservableCollection<DrawingInfo> _drawings = new ObservableCollection<DrawingInfo>();

        public ObservableCollection<DrawingInfo> Drawings { get { return _drawings; } }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog d = new OpenFileDialog();

                if (d.ShowDialog() ?? false)
                {
                    DirectoryInfo di = new DirectoryInfo(d.FileName);
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
                DirectoryInfo di = new DirectoryInfo(txtPath.Text);

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

                if (fi.Extension.ToLower() == ".dwg" || fi.Extension.ToLower() == ".pdf" || fi.Extension.ToLower() == ".tif" || fi.Extension.ToLower() == ".xls" || fi.Extension.ToLower() == ".xlsx")
                {
                    DrawingInfo dri = new DrawingInfo();
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
                var selectedDrawings = (from d in _drawings where d.Include == true select d);

                foreach (DrawingInfo di in selectedDrawings)
                {
                    var dwgs = (from MPDrawing d in dc.Drawings where d.Number.ToLower() == di.Name.ToLower() select d);
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
