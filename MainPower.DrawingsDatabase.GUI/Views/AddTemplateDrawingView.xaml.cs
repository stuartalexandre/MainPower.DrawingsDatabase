using MainPower.DrawingsDatabase.Gui.ViewModels;
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

namespace MainPower.DrawingsDatabase.Gui.Views
{
    /// <summary>
    /// Interaction logic for AddTemplateDrawingView.xaml
    /// </summary>
    public partial class AddTemplateDrawingView : Window
    {
        public AddTemplateDrawingView()
        {
            InitializeComponent();
        }

        public bool? ShowDialog(MPDrawing d)
        {
            AddTemplateDrawingViewModel model = DataContext as AddTemplateDrawingViewModel;
            model.SelectedTemplate = model.RecentTemplates.Where(t => t.Path == "(default)").First();
            model.SelectedTemplate.Drawing = d;
            return this.ShowDialog();
        }
    }
}
