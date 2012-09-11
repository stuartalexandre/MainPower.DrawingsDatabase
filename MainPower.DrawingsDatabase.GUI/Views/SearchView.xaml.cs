using System.Windows.Controls;
using System.Windows.Input;
using MainPower.DrawingsDatabase.Gui.ViewModels;

namespace MainPower.DrawingsDatabase.Gui.Views
{
    /// <summary>
    /// Interaction logic for SearchDB.xaml
    /// </summary>
    public sealed partial class SearchView : Page
    {
        public SearchView()
        {
            InitializeComponent();
        }

        public void DrawingDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //this is not very mvvm ish, but we dont have dynamic available to us in .net35
            SearchViewModel dc = DataContext as SearchViewModel;
            dc.DrawingDoubleClick.Execute(null);
        }
    }
}
