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
using System.Data.SqlClient;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;
using System.Collections.ObjectModel;
using HC.Utils.Controls;
using HC.Utils;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for ViewDrawingWindow.xaml
    /// </summary>
    public sealed partial class ViewDrawingWindow : Window
    {
        private MPDrawing _d;
        private Action _refresh;
        private DrawingsDataContext _dc;
        private bool _enableEditByDefualt;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawing">The drawing to edit</param>
        /// <param name="refresh">Some action to perform if the drawing is changed</param>
        /// <param name="dc"></param>
        public ViewDrawingWindow(MPDrawing drawing, Action refresh, bool enableEditByDefualt = false)
        {
            InitializeComponent();
            
            _refresh = refresh;
            _dc = DBCommon.NewDC;
            _d = (from dwg in _dc.Drawings where dwg.Id == drawing.Id select dwg).First();
            _enableEditByDefualt = enableEditByDefualt;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            propGrid.Instance = _d;
            if (_enableEditByDefualt)
                Button_Click(null, null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            btnSave.Focus();//prop grid wont save unless it loses focus (which it wont if we press enter instead of click the button)
            if (propGrid.IsReadOnly)
            {
                propGrid.IsReadOnly = false;
                btnSave.Content = "Save and Close";
                lblEditMessage.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {

                if (DBCommon.DrawingComboExists(_d.Number, _d.Sheet, _d.Id))
                {
                    MessageBox.Show("A drawing with the specified number and sheet number already exists.");
                }
                else
                {
                    try
                    {
                        _dc.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error has occured while trying to commit changes to the database.  A data concurrency exception may have occured, please refresh the data by re-searching." + Environment.NewLine + Environment.NewLine + ex.Message);
                    }
                    this.Close();
                }
            }
        }

        private void ViewPage_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_refresh != null) _refresh();
        }
    }
    
}


