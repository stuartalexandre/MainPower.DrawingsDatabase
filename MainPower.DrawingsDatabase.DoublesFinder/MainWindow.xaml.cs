using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;

namespace MainPower.DrawingsDatabase.DoublesFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DrawingsDataContext _dc = DBCommon.NewDC;
        private readonly ObservableCollection<MPDrawing> _drawings = new ObservableCollection<MPDrawing>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public IEnumerable<Drawing> DrawingItems
        {
            get { return _drawings; }
        }

        private void btnFindDuplicates_Click(object sender, RoutedEventArgs e)
        {
            _drawings.Clear();

            IQueryable<Drawing> allDrawings = from MPDrawing d in _dc.Drawings select d;

            var results = new List<MPDrawing>();

            foreach (MPDrawing d in allDrawings)
            {
                Drawing d1 = d;
                IQueryable<Drawing> search = from d2 in _dc.Drawings
                                             where d2.Number == d1.Number && d2.Sheet == d1.Sheet && d2.Id != d1.Id
                                             select d2;

                if (search.Any())
                {
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
            MessageBoxResult res =
                MessageBox.Show(
                    "Deleting a record cannot be undone.  Are you sure you want to delete " +
                    listBox1.SelectedItems.Count + " records?", "Really?", MessageBoxButton.YesNo);
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