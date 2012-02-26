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
using System.Collections.ObjectModel;
using MainPower.DrawingsDatabase.DatabaseHelper;
using System.Data.SqlClient;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Dynamic;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;
using System.Data.Linq.Mapping;
using System.IO;
using System.Diagnostics;
using System.Linq.Expressions;
using HC.Utils.Controls;
using HC.Utils;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Markup;


namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Interaction logic for SearchDB.xaml
    /// </summary>
    public sealed partial class SearchView : Page, IListViewPrinter
    {
        #region Printing Code

        //private delegate void MethodInvoker();

        public XpsDocument CreateXps(double pageWidth, double pageHeight, string tmpFileName)
        {
            FlowDocument flowDoc = new FlowDocument();
            Table table1 = new Table();
            flowDoc.Blocks.Add(table1);
            // Set some global formatting properties for the table.
            table1.CellSpacing = 5;
            table1.Background = Brushes.White;

            double sumOfColumnWidths = 0;

            ListViewPrintInfo pi = listView1.GetListViewPrintInfo();

            foreach (ColumnInfo g in pi.Columns)
            {
                sumOfColumnWidths += g.Width;
            }

            foreach (ColumnInfo g in pi.Columns)
            {
                table1.Columns.Add(new TableColumn() { Width = new GridLength(g.Width / sumOfColumnWidths * pageWidth) });
            }
            // Create and add an empty TableRowGroup to hold the table's Rows.
            table1.RowGroups.Add(new TableRowGroup());
            table1.RowGroups[0].Rows.Add(new TableRow());
            TableRow currentRow = table1.RowGroups[0].Rows[0];
            // Global formatting for the title row.
            currentRow.Background = Brushes.Silver;
            currentRow.FontSize = 20;
            currentRow.FontWeight = System.Windows.FontWeights.Bold;
            // Add the header row with content, 
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Drawings database search results"))));
            // and set the row to span all 6 columns.
            currentRow.Cells[0].ColumnSpan = table1.Columns.Count;

            // Add the second (header) row.
            table1.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table1.RowGroups[0].Rows[1];

            // Global formatting for the header row.
            currentRow.FontSize = 11;
            currentRow.FontWeight = FontWeights.Normal;

            foreach (ColumnInfo g in pi.Columns)
            {
                Run r = new Run(g.Header);
                currentRow.Cells.Add(new TableCell(new Paragraph(r)));
            }
            //add rows
            foreach (MPDrawing d in pi.Data)
            {
                // Add the next row.
                table1.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table1.RowGroups[0].Rows[table1.RowGroups[0].Rows.Count - 1];

                // Global formatting for the row.
                currentRow.FontSize = 8;
                currentRow.FontWeight = FontWeights.Normal;

                foreach (ColumnInfo g in pi.Columns)
                {
                    // Add cells with content to the third row.
                    object obj = typeof(MPDrawing).GetProperty(g.Header).GetValue(d, null);
                    string text = "";
                    if (obj is DateTime)
                    {
                        text = ((DateTime)obj).ToShortDateString();
                    }
                    else if (obj != null)
                    {
                        text = obj.ToString();
                    }
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run(text))));
                }

            }

            DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDoc).DocumentPaginator;
            paginator.PageSize = new Size(pageWidth, pageHeight);
            flowDoc.PagePadding = new Thickness(25);
            flowDoc.ColumnWidth = double.PositiveInfinity;

            try
            {
                // write the XPS document
                using (XpsDocument doc = new XpsDocument(tmpFileName, FileAccess.ReadWrite))
                {
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                    writer.Write(paginator);
                }

                // Read the XPS document into a dynamically generated
                // preview Window 
                XpsDocument document = new XpsDocument(tmpFileName, FileAccess.Read);

                return document;
            }
            catch { return null; }
        }
    
        #endregion

        private ObservableCollection<MPDrawing> _searchresults = new ObservableCollection<MPDrawing>();
        public ObservableCollection<MPDrawing> SearchResults { get { return _searchresults; } }

        public SearchView()
        {
            InitializeComponent();
        }

        private void AddSortBinding()
        {
            GridView gv = (GridView)listView1.View;

            for (int i = 1; i <= gv.Columns.Count; i++)
            {
                GridViewColumn col = gv.Columns[i - 1];
                ListViewSorter.SetSortBindingMember(col, new Binding((string)col.Header));
            }
        }

        #region Search Code

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _searchresults.Clear();
                DrawingsDataContext dc = DBCommon.NewDC;
                TextSearchOption options = (TextSearchOption)cmbTextMatch.SelectedIndex;
                IQueryable<MPDrawing> query = dc.Drawings;

                if (!String.IsNullOrEmpty(txtDrawingNumber.Text))
                {
                    if (chkLegacyNumbers.IsChecked ?? false)
                    {
                        query = query.Where(d => d.Number.Contains(txtDrawingNumber.Text) || d.LegacyDrawing.Contains(txtDrawingNumber.Text));
                    }
                    else
                    {
                        query = query.Where(d => d.Number.Contains(txtDrawingNumber.Text));
                    }
                }
                if (!String.IsNullOrEmpty(txtProjectTitle.Text))
                {
                    query = query.Where(SearchAssistant.SearchText(txtProjectTitle.Text, options, d => d.ProjectTitle));
                }
                if (chkElectronic.IsChecked ?? false)
                {
                    query = query.Where(d => d.Electronic ?? false);
                }
                if (chkNotElectronic.IsChecked ?? false)
                {
                    query = query.Where(d => d.Electronic == false);
                }
                if (chkOneTitle.IsChecked ?? false)
                {
                    if (!String.IsNullOrEmpty(txtTitle1.Text))
                    {
                        query = query.Where(SearchAssistant.SearchText(txtTitle1.Text, options, d => d.TitleLine1, d => d.TitleLine2, d => d.TitleLine3));
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(txtTitle1.Text))
                    {
                        query = query.Where(SearchAssistant.SearchText(txtTitle1.Text, options, d => d.TitleLine1));
                    }
                    if (!String.IsNullOrEmpty(txtTitle2.Text))
                    {
                        query = query.Where(SearchAssistant.SearchText(txtTitle2.Text, options, d => d.TitleLine2));
                    }
                    if (!String.IsNullOrEmpty(txtTitle3.Text))
                    {
                        query = query.Where(SearchAssistant.SearchText(txtTitle3.Text, options, d => d.TitleLine3));
                    }
                }
                if (!chkCategoryAll.IsChecked ?? false)
                {

                    Expression<Func<MPDrawing, bool>> w = (d => d.Category == DrawingCategory.Undefined);

                    if (chkComms.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.Communications);
                    }
                    if (chkGXP.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.GxpSubstation);
                    }
                    if (chkMisc.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.Miscellaneous);
                    }
                    if (chkOverhead.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.Overhead);
                    }
                    if (chkUnderground.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.Underground);
                    }
                    if (chkZoneSub.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.ZoneSubstation);
                    }
                    if (chkGeneration.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.Generation);
                    }

                    query = query.Where(w);
                }
                if (!chkStatusAll.IsChecked ?? false)
                {
                    Expression<Func<MPDrawing, bool>> w = (d => d.Status == DrawingStatus.Undefined);

                    if (chkSuperseded.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.Superseded);
                    }
                    if (chkPlanned.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.Planned);
                    }
                    if (chkCancelled.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.Canceled);
                    }
                    if (chkAsBuilt.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.AsBuilt);
                    }
                    if (chkCurrent.IsChecked ?? false)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.Current);
                    }
                    query = query.Where(w);
                }
                if (chkDate.IsChecked ?? false)
                {
                    query = query.Where(d => d.DrawnDate > DateTime.Parse(txtDateFrom.Text) && d.DrawnDate < DateTime.Parse(txtDateTo.Text));
                }

                foreach (MPDrawing d in query.ToList())
                {
                    _searchresults.Add(d);
                }
                if (listView1.Items.Count > 0) listView1.ScrollIntoView(listView1.Items.GetItemAt(0));
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReallyAdvSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _searchresults.Clear();

                DrawingsDataContext dc = DBCommon.NewDC;
                IQueryable<MPDrawing> query = dc.Drawings;

                query = query.Where(txtAdvSearch.Text);

                foreach (MPDrawing d in query.ToList())
                {
                    _searchresults.Add(d);
                }
                if (listView1.Items.Count > 0) listView1.ScrollIntoView(listView1.Items.GetItemAt(0));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Controls Logic

        private void expSearch_Expanded(object sender, RoutedEventArgs e)
        {
            //for some reason this event fires before the entire window is loaded...
            if (expReallyAdvcSearch != null)
            {
                expReallyAdvcSearch.IsExpanded = false;
            }
        }

        private void expReallyAdvcSearch_Expanded(object sender, RoutedEventArgs e)
        {
            expSearch.IsExpanded = false;
        }

        private void mnuChooseColumns(object sender, RoutedEventArgs e)
        {
            Dictionary<Type, IValueConverter> dict = new Dictionary<Type, IValueConverter>();
            dict.Add(typeof(DateTime), new DateConverter());
            dict.Add(typeof(DrawingCategory), new DrawingCategoryConverter());
            dict.Add(typeof(DateTime?), new DateConverter());
            dict.Add(typeof(DrawingStatus), new DrawingStatusConverter());
            ColumnChooser c = new ColumnChooser(typeof(MPDrawing), listView1, (DataTemplate)FindResource("HeaderTemplateSortNon"), dict);
            c.ShowDialog();
            AddSortBinding();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtTitle1.Text = "";
            txtTitle2.Text = "";
            txtTitle3.Text = "";
            txtProjectTitle.Text = "";
            txtDrawingNumber.Text = "";
            txtDateTo.Text = "";
            txtDateFrom.Text = "";
            chkDate.IsChecked = false;
            chkCategoryAll.IsChecked = true;
            chkStatusAll.IsChecked = true;
            cmbTextMatch.SelectedIndex = 1;//must contain all words
            chkOneTitle.IsChecked = false;
        }

        #endregion

        #region Drawing Actions

        private void mnuOpenDrawing_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MPDrawing d = (MPDrawing)listView1.SelectedItem;
                DBCommon.OpenAutoCadDrawing(d.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView1.SelectedItem == null) return;
            ViewDrawingWindow win = new ViewDrawingWindow((MPDrawing)listView1.SelectedItem, listView1.Items.Refresh);
            win.Show();
            //set this to true, otherwise someone else will mess with our popup window... (im looking at you listview)
            e.Handled = true;
        }

        private void mnuOpenInExplorer_Click(object sender, RoutedEventArgs e)
        {
            MPDrawing dwg = listView1.SelectedItem as MPDrawing;
            if (dwg != null)
            {
                Process.Start("explorer.exe", "/select,\"" + dwg.FileName + "\"");
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                mnuDeleteDrawing.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                mnuDeleteDrawing.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void mnuDeleteDrawing_Click(object sender, RoutedEventArgs e)
        {
            MPDrawing dwg = listView1.SelectedItem as MPDrawing;
            if (dwg != null)
            {
                if (MessageBox.Show("Deleting a drawing cannot be undone.  Are you sure you want to do this?", "Willy nilly drawing deleting is bad.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DBCommon.DeleteDrawing(dwg.Id);
                    _searchresults.Remove(dwg);
                }
            }
        }
        #endregion


        //TODO: should rename this to persistGUIsettings or something...
        internal void PersistColumnLayout()
        {
            Properties.Settings.Default.SearchColumns = XamlWriter.Save(((GridView)listView1.View).Columns);
            //make up a string in the format
            //"1,1,0,1,0,1,0,0,0,1,0,1,1,0,1" which is the status of the checkboxes.
            Properties.Settings.Default.Save();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string xaml = Properties.Settings.Default.SearchColumns;
            if (!String.IsNullOrEmpty(xaml))
            {
                //listView1.View = XamlReader.Parse(xaml) as GridView;

                GridViewColumnCollection savedCols = XamlReader.Parse(xaml) as GridViewColumnCollection;

                GridViewColumnCollection cols = ((GridView)listView1.View).Columns;

                List<GridViewColumn> theCols = new List<GridViewColumn>();

                foreach (GridViewColumn col in savedCols)
                {
                    theCols.Add(col);
                }
                
                cols.Clear();
                savedCols.Clear();

                foreach (GridViewColumn col in theCols)
                {
                    col.HeaderTemplate = listView1.TryFindResource("HeaderTemplateSortNon") as DataTemplate;
                    cols.Add(col);
                }
            }
            
            AddSortBinding();
        }
    }
}
