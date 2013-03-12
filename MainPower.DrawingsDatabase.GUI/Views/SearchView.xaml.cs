using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using MainPower.DrawingsDatabase.Gui.ViewModels;

namespace MainPower.DrawingsDatabase.Gui.Views
{
    /// <summary>
    /// Interaction logic for SearchDB.xaml
    /// </summary>
    public sealed partial class SearchView : Page
    {
        private bool _loaded = false;

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



        /// <summary>
        /// When the page is loaded, retrieve the persisted column layout
        /// This is a pretty grubby way of doing things but I couldn't think
        /// of anything better :/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //this event will fire not only when is first shown, but every time its shown
            // eg when switching between tabs.  Not sure why
            if (_loaded)
                return;
            
            //fetch the layout from settings
            string xaml = Properties.Settings.Default.SearchColumns;
            
            if (!String.IsNullOrEmpty(xaml))
            {
                GridViewColumnCollection savedCols = XamlReader.Parse(xaml) as GridViewColumnCollection;
                GridViewColumnCollection gridViewCols = ((GridView)listView1.View).Columns;
                
                //we can't share GridViewColumns between collections, nor can we 
                //remove one during a foreach, so we have to copy them to a list,
                //then add them to the collection.
                List<GridViewColumn> tempCols = new List<GridViewColumn>();

                foreach (GridViewColumn col in savedCols) { tempCols.Add(col); }
                
                gridViewCols.Clear();
                savedCols.Clear();

                //TODO: we should be using the existing converter resource rather than making a new one
                BooleanToVisibilityConverter boolToVisCon = new BooleanToVisibilityConverter();

                foreach (GridViewColumn col in tempCols)
                {
                    //add the templates, bindings, and set the visible states as these aren't persisted
                    col.HeaderTemplate = listView1.TryFindResource("HeaderTemplateSortNon") as DataTemplate;
                    GridViewColumnHeader gvch = col.Header as GridViewColumnHeader;
                    MenuItem mnu = this.FindName("mnuCol" + gvch.Content.ToString()) as MenuItem;
                    mnu.IsChecked = (gvch.Visibility == System.Windows.Visibility.Visible);
                    Binding b = new Binding("IsChecked");
                    b.ElementName = "mnuCol" + gvch.Content.ToString();
                    b.Converter = boolToVisCon;
                    gvch.SetBinding(UIElement.VisibilityProperty, b);
                    gridViewCols.Add(col);
                }
            }
            _loaded = true;

        }

        /// <summary>
        /// Save the column layout of lvResults to the application settings.
        /// </summary>
        public void PersistColumns()
        {
            Properties.Settings.Default.SearchColumns = XamlWriter.Save(((GridView)listView1.View).Columns);
        }
    }
}
