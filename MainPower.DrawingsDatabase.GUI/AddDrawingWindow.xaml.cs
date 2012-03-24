using System.Windows;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for AddDrawing.xaml
    /// </summary>
    public sealed partial class AddDrawingWindow
    {
        private readonly MPDrawing _d = DBCommon.CreateDefaultDrawing();

        public AddDrawingWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DBCommon.DrawingComboExists(_d.Number, _d.Sheet))
            {
                MessageBox.Show(
                    "A drawing with the specified number and sheet number already exists, or the Drawing Number or Sheet Number was null.  This is not acceptable.");
            }
            else
            {
                DrawingsDataContext dc = DBCommon.NewDC;
                dc.Drawings.InsertOnSubmit(_d);
                dc.SubmitChanges();
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            propGrid.Instance = _d;
        }
    }
}