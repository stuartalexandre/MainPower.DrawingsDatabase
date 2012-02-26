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
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;
using System.Collections.ObjectModel;
using HC.Utils.Controls;
using HC.Utils;
using MainPower.DrawingsDatabase.DatabaseHelper;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for AddDrawing.xaml
    /// </summary>
    public sealed partial class AddDrawingWindow : Window
    {
        private MPDrawing _d = DBCommon.CreateDefaultDrawing();

        public AddDrawingWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DBCommon.DrawingComboExists(_d.Number, _d.Sheet))
            {
                MessageBox.Show("A drawing with the specified number and sheet number already exists, or the Drawing Number or Sheet Number was null.  This is not acceptable.");
            }
            else
            {
                DrawingsDataContext dc = DBCommon.NewDC;
                dc.Drawings.InsertOnSubmit(_d);
                dc.SubmitChanges();
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            propGrid.Instance = _d;
        }
    }
}
