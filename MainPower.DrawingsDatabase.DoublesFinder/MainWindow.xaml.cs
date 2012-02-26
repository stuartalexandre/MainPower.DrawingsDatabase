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
using MainPower.DrawingsDatabase.DatabaseHelper;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;
using System.Collections.ObjectModel;

namespace MainPower.DrawingsDatabase.DoublesFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<MPDrawing> DrawingItems
        {
            get { return _drawings; }
        }

        private ObservableCollection<MPDrawing> _drawings = new ObservableCollection<MPDrawing>();

        private DrawingsDataContext _dc = DBCommon.NewDC;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnFindDuplicates_Click(object sender, RoutedEventArgs e)
        {
            _drawings.Clear();

            var allDrawings = from MPDrawing d in _dc.Drawings select d;

            List<MPDrawing> results = new List<MPDrawing>();

            foreach (MPDrawing d in allDrawings)
            {
                var search = from d2 in _dc.Drawings where d2.Number == d.Number && d2.Sheet == d.Sheet && d2.Id != d.Id select d2;

                if (search.Count() > 0){
                    foreach (MPDrawing d3 in search)
                    {
                        if (!results.Contains(d3))
                        {
                            results.Add(d3);
                        }
                    }
                    if (!results.Contains(d))
                    {
                        results.Add(d);
                    }
                }
            }


            foreach (MPDrawing d in results)
            {
                _drawings.Add(d);
            }
        }

        private void btnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Deleting a record cannot be undone.  Are you sure you want to delete " + listBox1.SelectedItems.Count + " records?", "Really?", MessageBoxButton.YesNo);
            if (res != MessageBoxResult.Yes)
                return;

            foreach (MPDrawing d in listBox1.SelectedItems)
            {
                _dc.Drawings.DeleteOnSubmit(d);
            }
            _dc.SubmitChanges();
        }
    }
}
