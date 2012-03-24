using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for ViewDrawingWindow.xaml
    /// </summary>
    public sealed partial class ViewDrawingWindow
    {
        private readonly MPDrawing _d;
        private readonly DrawingsDataContext _dc;
        private readonly bool _enableEditByDefualt;
        private readonly Action _refresh;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawing">The drawing to edit</param>
        /// <param name="refresh">Some action to perform if the drawing is changed</param>
        /// <param name="enableEditByDefualt"> </param>
        public ViewDrawingWindow(MPDrawing drawing, Action refresh, bool enableEditByDefualt = false)
        {
            InitializeComponent();

            _refresh = refresh;
            _dc = DBCommon.NewDC;
            _d = (from dwg in _dc.Drawings where dwg.Id == drawing.Id select dwg).First();
            _enableEditByDefualt = enableEditByDefualt;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            propGrid.Instance = _d;
            if (_enableEditByDefualt)
                ButtonClick(null, null);
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            btnSave.Focus();
                //prop grid wont save unless it loses focus (which it wont if we press enter instead of click the button)
            if (propGrid.IsReadOnly)
            {
                propGrid.IsReadOnly = false;
                btnSave.Content = "Save and Close";
                lblEditMessage.Visibility = Visibility.Visible;
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
                        MessageBox.Show(
                            "An error has occured while trying to commit changes to the database.  A data concurrency exception may have occured, please refresh the data by re-searching." +
                            Environment.NewLine + Environment.NewLine + ex.Message);
                    }
                    Close();
                }
            }
        }

        private void ViewPageClosing(object sender, CancelEventArgs e)
        {
            if (_refresh != null) _refresh();
        }
    }
}