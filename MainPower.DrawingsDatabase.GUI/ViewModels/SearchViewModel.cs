using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using HC.Utils;
using HC.Utils.Controls;
using MainPower.DrawingsDatabase.DatabaseHelper;
using Drawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;
using MicroMvvm;

namespace MainPower.DrawingsDatabase.Gui.ViewModels
{
    public class SearchViewModel : ViewModelBase//, IListViewPrinter
    {

#region Fields

        private ObservableCollection<Drawing> _searchresults = new ObservableCollection<Drawing>();

        private string _drawingNumber;
        private string _title1;
        private string _title2;
        private string _title3;
        private string _projectTitle;
        private DateTime _startDate;
        private DateTime _endDate;
        private TextSearchOption _searchType;
        private bool _includeElectronic;
        private bool _includeNonElectronic;
        private bool _searchAllTitles;
        private bool _includeLegacyNumbers;
        private bool _searchDateRange;
        private bool _categoryAll;
        private bool _categoryMiscellaneous;
        private bool _categoryOverhead;
        private bool _categoryGXPSubstation;
        private bool _categoryZoneSubstation;
        private bool _categoryUnderground;
        private bool _categoryCommunications;
        private bool _categoryGeneration;
        private bool _statusAll;
        private bool _statusAsBuilt;
        private bool _statusCancelled;
        private bool _statusSuperseded;
        private bool _statusPlanned;
        private bool _statusCurrent;
        private Drawing _selectedDrawing;
        private string _advancedSearchQuery;
#endregion

#region Properties

        public string DrawingNumber
        {
            get { return _drawingNumber; }
            set
            {
                if (DrawingNumber == value) return;
                _drawingNumber = value;
                OnPropertyChanged("DrawingNumber");
            }
        }
        public string Title1
        {
            get { return _title1; }
            set
            {
                if (Title1 == value) return;
                _title1 = value;
                OnPropertyChanged("Title1");
            }
        }
        public string Title2
        {
            get { return _title2; }
            set
            {
                if (Title2 == value) return;
                _title2 = value;
                OnPropertyChanged("Title2");
            }
        }
        public string Title3
        {
            get { return _title3; }
            set
            {
                if (Title3 == value) return;
                _title3 = value;
                OnPropertyChanged("Title3");
            }
        }
        public string ProjectTitle
        {
            get { return _projectTitle; }
            set
            {
                if (ProjectTitle == value) return;
                _projectTitle = value;
                OnPropertyChanged("ProjectTitle");
            }
        }
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (StartDate == value) return;
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (EndDate == value) return;
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }
        public TextSearchOption SearchType
        {
            get { return _searchType; }
            set
            {
                if (SearchType == value) return;
                _searchType = value;
                OnPropertyChanged("SearchType");
            }
        }
        public bool IncludeElectronic
        {
            get { return _includeElectronic; }
            set
            {
                if (IncludeElectronic == value) return;
                _includeElectronic = value;
                OnPropertyChanged("IncludeElectronic");
            }
        }
        public bool IncludeNonElectronic
        {
            get { return _includeNonElectronic; }
            set
            {
                if (IncludeNonElectronic == value) return;
                _includeNonElectronic = value;
                OnPropertyChanged("IncludeNonElectronic");
            }
        }
        public bool SearchAllTitles
        {
            get { return _searchAllTitles; }
            set
            {
                if (SearchAllTitles == value) return;
                _searchAllTitles = value;
                OnPropertyChanged("SearchAllTitles");
            }
        }
        public bool IncludeLegacyNumbers
        {
            get { return _includeLegacyNumbers; }
            set
            {
                if (IncludeLegacyNumbers == value) return;
                _includeLegacyNumbers = value;
                OnPropertyChanged("IncludeLegacyNumbers");
            }
        }
        public bool SearchDateRange
        {
            get { return _searchDateRange; }
            set
            {
                if (SearchDateRange == value) return;
                _searchDateRange = value;
                OnPropertyChanged("SearchDateRange");
            }
        }
        public bool CategoryAll 
        {
            get { return _categoryAll; }
            set
            {
                if (CategoryAll == value) return;
                _categoryAll = value;
                OnPropertyChanged("CategoryAll");
            }
        }
        public bool CategoryMiscellaneous 
        {
            get { return _categoryMiscellaneous; }
            set
            {
                if (CategoryMiscellaneous == value) return;
                _categoryMiscellaneous = value;
                OnPropertyChanged("CategoryMiscellaneous");
            }
        }
        public bool CategoryOverhead
        {
            get { return _categoryOverhead; }
            set
            {
                if (CategoryOverhead == value) return;
                _categoryOverhead = value;
                OnPropertyChanged("CategoryOverhead");
            }
        }
        public bool CategoryGXPSubstation
        {
            get { return _categoryGXPSubstation; }
            set
            {
                if (CategoryGXPSubstation == value) return;
                _categoryGXPSubstation = value;
                OnPropertyChanged("CategoryGXPSubstation");
            }
        }
        public bool CategoryZoneSubstation
        { 
            get { return _categoryZoneSubstation; }
            set
            {
                if (CategoryZoneSubstation == value) return;
                _categoryZoneSubstation = value;
                OnPropertyChanged("CategoryZoneSubstation");
            }
        }
        public bool CategoryUnderground
        {
            get { return _categoryUnderground; }
            set
            {
                if (CategoryUnderground == value) return;
                _categoryUnderground = value;
                OnPropertyChanged("CategoryUnderground");
            }
        }
        public bool CategoryCommunications
        {
            get { return _categoryCommunications; }
            set
            {
                if (CategoryCommunications == value) return;
                _categoryCommunications = value;
                OnPropertyChanged("CategoryCommunications");
            }
        }
        public bool CategoryGeneration
        {
            get { return _categoryGeneration; }
            set
            {
                if (CategoryGeneration == value) return;
                _categoryGeneration = value;
                OnPropertyChanged("CategoryGeneration");
            }
        }
        public bool StatusAll
        {
            get { return _statusAll; }
            set
            {
                if (StatusAll == value) return;
                _statusAll = value;
                OnPropertyChanged("StatusAll");
            }
        }
        public bool StatusAsBuilt
        {
            get { return _statusAsBuilt; }
            set
            {
                if (StatusAsBuilt == value) return;
                _statusAsBuilt = value;
                OnPropertyChanged("StatusAsBuilt");
            }
        }
        public bool StatusCancelled
        {
            get { return _statusCancelled; }
            set
            {
                if (StatusCancelled == value) return;
                _statusCancelled = value;
                OnPropertyChanged("StatusCancelled");
            }
        }
        public bool StatusSuperseded
        {
            get { return _statusSuperseded; }
            set
            {
                if (StatusSuperseded == value) return;
                _statusSuperseded = value;
                OnPropertyChanged("StatusSuperseded");
            }
        }
        public bool StatusPlanned
        {
            get { return _statusPlanned; }
            set
            {
                if (StatusPlanned == value) return;
                _statusPlanned = value;
                OnPropertyChanged("StatusPlanned");
            }
        }
        public bool StatusCurrent
        {
            get { return _statusCurrent; }
            set
            {
                if (StatusCurrent == value) return;
                _statusCurrent = value;
                OnPropertyChanged("StatusCurrent");
            }
        }
        public String AdvancedSearchQuery
        {
            get { return _advancedSearchQuery; }
            set
            {
                if (AdvancedSearchQuery == value) return;
                _advancedSearchQuery= value;
                OnPropertyChanged("AdvancedSearchQuery");
            }
        }
        public Drawing SelectedDrawing
        {
            get { return _selectedDrawing; }
            set
            {
                if (SelectedDrawing== value) return;
                _selectedDrawing= value;
                OnPropertyChanged("SelectedDrawing");
            }
        }
        public ObservableCollection<Drawing> SearchResults { get { return _searchresults; } }

#endregion

#region Misc code to sort out
        /*public XpsDocument CreateXps(double pageWidth, double pageHeight, string tmpFileName)
        {
            var flowDoc = new FlowDocument();
            var table1 = new Table();
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
            foreach (Drawing d in pi.Data)
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
                    object obj = typeof(Drawing).GetProperty(g.Header).GetValue(d, null);
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

        private void AddSortBinding()
        {
            GridView gv = (GridView)listView1.View;

            for (int i = 1; i <= gv.Columns.Count; i++)
            {
                GridViewColumn col = gv.Columns[i - 1];
                ListViewSorter.SetSortBindingMember(col, new Binding((string)col.Header));
            }
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
        }*/

#endregion

#region Commands
        void SearchExecute()
        {
             try
            {
                _searchresults.Clear();
                DrawingsDataContext dc = DBCommon.NewDC;
                IQueryable<Drawing> query = dc.Drawings;

                if (!String.IsNullOrEmpty(DrawingNumber))
                {
                    if (IncludeLegacyNumbers)
                    {
                        query = query.Where(d => d.Number.Contains(DrawingNumber) || d.LegacyDrawing.Contains(DrawingNumber));
                    }
                    else
                    {
                        query = query.Where(d => d.Number.Contains(DrawingNumber));
                    }
                }
                if (!String.IsNullOrEmpty(ProjectTitle))
                {
                    query = query.Where(SearchAssistant.SearchText(ProjectTitle, SearchType, d => d.ProjectTitle));
                }
                if (IncludeElectronic)
                {
                    query = query.Where(d => d.Electronic == true);
                }
                if (IncludeNonElectronic)
                {
                    query = query.Where(d => d.Electronic == false);
                }
                if (SearchAllTitles)
                {
                    if (!String.IsNullOrEmpty(Title1))
                    {
                        query = query.Where(SearchAssistant.SearchText(Title1, SearchType, d => d.TitleLine1, d => d.TitleLine2, d => d.TitleLine3));
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(Title1))
                    {
                        query = query.Where(SearchAssistant.SearchText(Title1, SearchType, d => d.TitleLine1));
                    }
                    if (!String.IsNullOrEmpty(Title2))
                    {
                        query = query.Where(SearchAssistant.SearchText(Title2, SearchType, d => d.TitleLine2));
                    }
                    if (!String.IsNullOrEmpty(Title3))
                    {
                        query = query.Where(SearchAssistant.SearchText(Title3, SearchType, d => d.TitleLine3));
                    }
                }
                if (!CategoryAll)
                {

                    Expression<Func<Drawing, bool>> w = (d => d.Category == DrawingCategory.Undefined);

                    if (CategoryCommunications)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.Communications);
                    }
                    if (CategoryGXPSubstation)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.GxpSubstation);
                    }
                    if (CategoryMiscellaneous)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.Miscellaneous);
                    }
                    if (CategoryOverhead)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.Overhead);
                    }
                    if (CategoryUnderground)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.Underground);
                    }
                    if (CategoryZoneSubstation)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.ZoneSubstation);
                    }
                    if (CategoryGeneration)
                    {
                        w = w.OrElse(d => d.Category == DrawingCategory.Generation);
                    }

                    query = query.Where(w);
                }
                if (!StatusAll)
                {
                    Expression<Func<Drawing, bool>> w = (d => d.Status == DrawingStatus.Undefined);

                    if (StatusSuperseded)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.Superseded);
                    }
                    if (StatusPlanned)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.Planned);
                    }
                    if (StatusCancelled)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.Canceled);
                    }
                    if (StatusAsBuilt)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.AsBuilt);
                    }
                    if (StatusCurrent)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.Current);
                    }
                    query = query.Where(w);
                }
                if (SearchDateRange)
                {
                    query = query.Where(d => d.DrawnDate > StartDate && d.DrawnDate < EndDate);
                }

                foreach (Drawing d in query.ToList())
                {
                    _searchresults.Add(d);
                }
                //if (listView1.Items.Count > 0) listView1.ScrollIntoView(listView1.Items.GetItemAt(0));
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool CanSearchExecute()
        {
            return true;
        }

        public ICommand Search { get { return new RelayCommand(SearchExecute, CanSearchExecute); } }

        void AdvancedSearchExecute()
        {
             try
            {
                _searchresults.Clear();

                DrawingsDataContext dc = DBCommon.NewDC;
                IQueryable<Drawing> query = dc.Drawings;

                query = query.Where(AdvancedSearchQuery);

                foreach (Drawing d in query.ToList())
                {
                    _searchresults.Add(d);
                }
//                if (listView1.Items.Count > 0) listView1.ScrollIntoView(listView1.Items.GetItemAt(0));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool CanAdvancedSearchExecute()
        {
            return true;
        }

        public ICommand AdvancedSearch { get { return new RelayCommand(AdvancedSearchExecute, CanAdvancedSearchExecute); } }

        void DrawingDoubleClickExecute()
        {
            
        }

        bool CanDrawingDoubleClickExecute()
        {
            return true;
        }

        public ICommand DrawingDoubleClick { get { return new RelayCommand(DrawingDoubleClickExecute, CanDrawingDoubleClickExecute); } }

        void ViewDrawingExecute()
        {
            if (SelectedDrawing == null) return;
            //ViewDrawingWindow win = new ViewDrawingWindow(SelectedDrawing);
            //win.Show();
        }

        bool CanViewDrawingExecute()
        {
            return true;
        }

        public ICommand ViewDrawing { get { return new RelayCommand(ViewDrawingExecute, CanViewDrawingExecute); } }

        void OpenDrawingExecute()
        {
            try
            {
                Drawing d = SelectedDrawing;
                DBCommon.OpenAutoCadDrawing(d.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool CanOpenDrawingExecute()
        {
            return true;
        }

        public ICommand OpenDrawing { get { return new RelayCommand(OpenDrawingExecute, CanOpenDrawingExecute); } }

        void DeleteDrawingExecute()
        {
            if (SelectedDrawing != null)
            {
                if (
                    MessageBox.Show("Deleting a drawing cannot be undone.  Are you sure you want to do this?",
                                    "Willy nilly drawing deleting is bad.", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                {
                    DBCommon.DeleteDrawing(SelectedDrawing.Id);
                    _searchresults.Remove(SelectedDrawing);
                }
            }
        }

        bool CanDeleteDrawingExecute()
        {
            return true;
        }

        public ICommand DeleteDrawing { get { return new RelayCommand(DeleteDrawingExecute, CanDeleteDrawingExecute); } }

        void OpenDrawingInExplorerExecute()
        {
            if (SelectedDrawing != null)
            {
                Process.Start("explorer.exe", "/select,\"" + SelectedDrawing.FileName + "\"");
            }
        }

        bool CanOpenDrawingInExplorerExecute()
        {
            return true;
        }

        public ICommand OpenDrawingInExplorer { get { return new RelayCommand(OpenDrawingInExplorerExecute, CanOpenDrawingInExplorerExecute); } }

        void ClearSearchExecute()
        {
            Title1 = "";
            Title2= "";
            Title3= "";
            ProjectTitle= "";
            DrawingNumber= "";
            StartDate = DateTime.Parse("1/1/1970");
            EndDate = DateTime.Parse("1/1/2050");
            SearchDateRange = false;
            CategoryAll = true;
            StatusAll = true;
            SearchType = TextSearchOption.SearchAllWords;
            SearchAllTitles = true;
        }

        bool CanClearSearchExecute()
        {
            return true;
        }

        public ICommand ClearSearch { get { return new RelayCommand(ClearSearchExecute, CanClearSearchExecute); } }

       
#endregion

    }
}
