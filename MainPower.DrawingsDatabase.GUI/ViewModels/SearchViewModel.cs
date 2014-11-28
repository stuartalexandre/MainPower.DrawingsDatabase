using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using MainPower.DrawingsDatabase.DatabaseHelper;
using Drawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;
using MicroMvvm;
using MainPower.DrawingsDatabase.Gui.Models;
using Microsoft.Win32;

namespace MainPower.DrawingsDatabase.Gui.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        
#region Fields
        private ObservableCollection<Drawing> _searchresults = new ObservableCollection<Drawing>();
        private DrawingSearchModel _searchFields = new DrawingSearchModel();
        private Drawing _selectedDrawing;
#endregion

#region Properties

        public Visibility CanDeleteDrawing
        {
            get
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public DrawingSearchModel Model
        {
            get
            {
                return _searchFields;
            }

            set
            {
                _searchFields = value;
                //we have to notify all the events that the model has changed...
                OnPropertyChanged(String.Empty);
            }
        }
        public string DrawingNumber
        {
            get { return _searchFields.DrawingNumber; }
            set
            {
                if (DrawingNumber == value) return;
                _searchFields.DrawingNumber = value;
                OnPropertyChanged("DrawingNumber");
            }
        }
        public string Title1
        {
            get { return _searchFields.Title1; }
            set
            {
                if (Title1 == value) return;
                _searchFields.Title1 = value;
                OnPropertyChanged("Title1");
            }
        }
        public string Title2
        {
            get { return _searchFields.Title2; }
            set
            {
                if (Title2 == value) return;
                _searchFields.Title2 = value;
                OnPropertyChanged("Title2");
            }
        }
        public string Title3
        {
            get { return _searchFields.Title3; }
            set
            {
                if (Title3 == value) return;
                _searchFields.Title3 = value;
                OnPropertyChanged("Title3");
            }
        }
        public string ProjectTitle
        {
            get { return _searchFields.ProjectTitle; }
            set
            {
                if (ProjectTitle == value) return;
                _searchFields.ProjectTitle = value;
                OnPropertyChanged("ProjectTitle");
            }
        }
        public DateTime StartDate
        {
            get { return _searchFields.StartDate; }
            set
            {
                if (StartDate == value) return;
                _searchFields.StartDate = value;
                OnPropertyChanged("StartDate");
            }
        }
        public DateTime EndDate
        {
            get { return _searchFields.EndDate; }
            set
            {
                if (EndDate == value) return;
                _searchFields.EndDate = value;
                OnPropertyChanged("EndDate");
            }
        }
        public TextSearchOption SearchType
        {
            get { return _searchFields.SearchType; }
            set
            {
                if (SearchType == value) return;
                _searchFields.SearchType = value;
                OnPropertyChanged("SearchType");
            }
        }
        public bool ElectronicOnly
        {
            get { return _searchFields.ElectronicOnly; }
            set
            {
                if (ElectronicOnly == value) return;
                _searchFields.ElectronicOnly = value;
                OnPropertyChanged("ElectronicOnly");
            }
        }
        public bool NonElectronicOnly
        {
            get { return _searchFields.NonElectronicOnly; }
            set
            {
                if (NonElectronicOnly == value) return;
                _searchFields.NonElectronicOnly = value;
                OnPropertyChanged("NonElectronicOnly");
            }
        }
        public bool StorageAgnostic
        {
            get { return _searchFields.StorageAgnostic; }
            set
            {
                if (StorageAgnostic == value) return;
                _searchFields.StorageAgnostic = value;
                OnPropertyChanged("StorageAgnostic");
            }
        }
        public bool SearchAllTitles
        {
            get { return _searchFields.SearchAllTitles; }
            set
            {
                if (SearchAllTitles == value) return;
                _searchFields.SearchAllTitles = value;
                OnPropertyChanged("SearchAllTitles");
            }
        }
        public bool IncludeLegacyNumbers
        {
            get { return _searchFields.IncludeLegacyNumbers; }
            set
            {
                if (IncludeLegacyNumbers == value) return;
                _searchFields.IncludeLegacyNumbers = value;
                OnPropertyChanged("IncludeLegacyNumbers");
            }
        }
        public bool SearchDateRange
        {
            get { return _searchFields.SearchDateRange; }
            set
            {
                if (SearchDateRange == value) return;
                _searchFields.SearchDateRange = value;
                OnPropertyChanged("SearchDateRange");
            }
        }
        public bool CategoryAll 
        {
            get { return _searchFields.CategoryAll; }
            set
            {
                if (CategoryAll == value) return;
                _searchFields.CategoryAll = value;
                OnPropertyChanged("CategoryAll");
            }
        }
        public bool CategoryMiscellaneous 
        {
            get { return _searchFields.CategoryMiscellaneous; }
            set
            {
                if (CategoryMiscellaneous == value) return;
                _searchFields.CategoryMiscellaneous = value;
                OnPropertyChanged("CategoryMiscellaneous");
            }
        }
        public bool CategoryOverhead
        {
            get { return _searchFields.CategoryOverhead; }
            set
            {
                if (CategoryOverhead == value) return;
                _searchFields.CategoryOverhead = value;
                OnPropertyChanged("CategoryOverhead");
            }
        }
        public bool CategoryGXPSubstation
        {
            get { return _searchFields.CategoryGXPSubstation; }
            set
            {
                if (CategoryGXPSubstation == value) return;
                _searchFields.CategoryGXPSubstation = value;
                OnPropertyChanged("CategoryGXPSubstation");
            }
        }
        public bool CategoryZoneSubstation
        {
            get { return _searchFields.CategoryZoneSubstation; }
            set
            {
                if (CategoryZoneSubstation == value) return;
                _searchFields.CategoryZoneSubstation = value;
                OnPropertyChanged("CategoryZoneSubstation");
            }
        }
        public bool CategoryUnderground
        {
            get { return _searchFields.CategoryUnderground; }
            set
            {
                if (CategoryUnderground == value) return;
                _searchFields.CategoryUnderground = value;
                OnPropertyChanged("CategoryUnderground");
            }
        }
        public bool CategoryCommunications
        {
            get { return _searchFields.CategoryCommunications; }
            set
            {
                if (CategoryCommunications == value) return;
                _searchFields.CategoryCommunications = value;
                OnPropertyChanged("CategoryCommunications");
            }
        }
        public bool CategoryGeneration
        {
            get { return _searchFields.CategoryGeneration; }
            set
            {
                if (CategoryGeneration == value) return;
                _searchFields.CategoryGeneration = value;
                OnPropertyChanged("CategoryGeneration");
            }
        }
        public bool StatusAll
        {
            get { return _searchFields.StatusAll; }
            set
            {
                if (StatusAll == value) return;
                _searchFields.StatusAll = value;
                OnPropertyChanged("StatusAll");
            }
        }
        public bool StatusAsBuilt
        {
            get { return _searchFields.StatusAsBuilt; }
            set
            {
                if (StatusAsBuilt == value) return;
                _searchFields.StatusAsBuilt = value;
                OnPropertyChanged("StatusAsBuilt");
            }
        }
        public bool StatusCancelled
        {
            get { return _searchFields.StatusCanceled; }
            set
            {
                if (StatusCancelled == value) return;
                _searchFields.StatusCanceled = value;
                OnPropertyChanged("StatusCancelled");
            }
        }
        public bool StatusSuperseded
        {
            get { return _searchFields.StatusSuperseded; }
            set
            {
                if (StatusSuperseded == value) return;
                _searchFields.StatusSuperseded = value;
                OnPropertyChanged("StatusSuperseded");
            }
        }
        public bool StatusPlanned
        {
            get { return _searchFields.StatusPlanned; }
            set
            {
                if (StatusPlanned == value) return;
                _searchFields.StatusPlanned = value;
                OnPropertyChanged("StatusPlanned");
            }
        }
        public bool StatusCurrent
        {
            get { return _searchFields.StatusCurrent; }
            set
            {
                if (StatusCurrent == value) return;
                _searchFields.StatusCurrent = value;
                OnPropertyChanged("StatusCurrent");
            }
        }
        public bool StatusDraft
        {
            get { return _searchFields.StatusDraft; }
            set
            {
                if (StatusDraft == value) return;
                _searchFields.StatusDraft = value;
                OnPropertyChanged("StatusDraft");
            }
        }
        public String AdvancedSearchQuery
        {
            get { return _searchFields.AdvancedSearchQuery; }
            set
            {
                if (AdvancedSearchQuery == value) return;
                _searchFields.AdvancedSearchQuery  = value;
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

        private void RefreshDrawings()
        {
            //TODO: fill this out it necessary, otherwise get rid of it
        }

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
                if (ElectronicOnly)
                {
                    query = query.Where(d => d.Electronic == true);
                }
                if (NonElectronicOnly)
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
                    if (StatusDraft)
                    {
                        w = w.OrElse(d => d.Status == DrawingStatus.Draft);
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

        void ExportSearchExecute()
        {
            try
            {
                SaveFileDialog d = new SaveFileDialog();
                if (d.ShowDialog() == true)
                {
                    using (StreamWriter sw = new StreamWriter(d.FileName))
                    {
                        sw.WriteLine("Approved By," +
                            "ApprovedByA," +
                            "ApprovedByB," +
                            "ApprovedByC," +
                            "ApprovedByD," +
                            "ApprovedByE," +
                            "ApprovedByF," +
                            "ApprovedByG," +
                            "ApprovedDate," +
                            "Category," +
                            "CheckedBy," +
                            "CheckedDate," +
                            "Comments," +
                            "Consultant," +
                            "DateRevA," +
                            "DateRevB," +
                            "DateRevC," +
                            "DateRevD," +
                            "DateRevE," +
                            "DateRevF," +
                            "DateRevG," +
                            "DescriptionRevA," +
                            "DescriptionRevB," +
                            "DescriptionRevC," +
                            "DescriptionRevD," +
                            "DescriptionRevE," +
                            "DescriptionRevF," +
                            "DescriptionRevG," +
                            "DrawnBy," +
                            "DrawnByRevA," +
                            "DrawnByRevB," +
                            "DrawnByRevC," +
                            "DrawnByRevD," +
                            "DrawnByRevE," +
                            "DrawnByRevF," +
                            "DrawnByRevG," +
                            "DrawnDate," +
                            "Electronic," +
                            "FileName," +
                            "Id," +
                            "LegacyDrawing," +
                            "Number," +
                            "ProjectTitle," +
                            "RevA," +
                            "RevB," +
                            "RevC," +
                            "RevD," +
                            "RevE," +
                            "RevF," +
                            "RevG," +
                            "Scale," +
                            "Sheet," +
                            "SheetRevision," +
                            "SheetSize," +
                            "Status," +
                            "TitleLine1," +
                            "TitleLine2," +
                            "TitleLine3," +
                            "TitleLine4" 
                            );
                        foreach (Drawing dw in _searchresults)
                        {
                            sw.WriteLine(string.Join(",", new string[]{
                                SwapNullForString(dw.ApprovedBy),
                                SwapNullForString(dw.ApprovedByRevA),
                                SwapNullForString(dw.ApprovedByRevB),
                                SwapNullForString(dw.ApprovedByRevC),
                                SwapNullForString(dw.ApprovedByRevD),
                                SwapNullForString(dw.ApprovedByRevE),
                                SwapNullForString(dw.ApprovedByRevF),
                                SwapNullForString(dw.ApprovedByRevG),
                                dw.ApprovedDate == null ? "" :dw.ApprovedDate.Value.ToShortDateString(),
                                Enum.GetName(typeof(DrawingCategory), dw.Category),
                                SwapNullForString(dw.CheckedBy),
                                dw.CheckedDate == null ? "" :dw.CheckedDate.Value.ToShortDateString(),
                                SwapNullForString(dw.Comments),
                                SwapNullForString(dw.Consultant),
                                dw.DateRevA == null ? "" : dw.DateRevA.Value.ToShortDateString(),
                                dw.DateRevB == null ? "" : dw.DateRevB.Value.ToShortDateString(),
                                dw.DateRevC == null ? "" : dw.DateRevC.Value.ToShortDateString(),
                                dw.DateRevD == null ? "" : dw.DateRevD.Value.ToShortDateString(),
                                dw.DateRevE == null ? "" : dw.DateRevE.Value.ToShortDateString(),
                                dw.DateRevF == null ? "" : dw.DateRevF.Value.ToShortDateString(),
                                dw.DateRevG == null ? "" : dw.DateRevG.Value.ToShortDateString(),
                                SwapNullForString(dw.DescriptionRevA),
                                SwapNullForString(dw.DescriptionRevB),
                                SwapNullForString(dw.DescriptionRevC),
                                SwapNullForString(dw.DescriptionRevD),
                                SwapNullForString(dw.DescriptionRevE),
                                SwapNullForString(dw.DescriptionRevF),
                                SwapNullForString(dw.DescriptionRevG),
                                SwapNullForString(dw.DrawnBy),
                                SwapNullForString(dw.DrawnByRevA),
                                SwapNullForString(dw.DrawnByRevB),
                                SwapNullForString(dw.DrawnByRevC),
                                SwapNullForString(dw.DrawnByRevD),
                                SwapNullForString(dw.DrawnByRevE),
                                SwapNullForString(dw.DrawnByRevF),
                                SwapNullForString(dw.DrawnByRevG),
                                dw.DrawnDate == null ? "" :dw.DrawnDate.Value.ToShortDateString(),
                                dw.Electronic.ToString(),
                                SwapNullForString(dw.FileName),
                                dw.Id.ToString(),
                                SwapNullForString(dw.LegacyDrawing),
                                SwapNullForString(dw.Number),
                                SwapNullForString(dw.ProjectTitle),
                                SwapNullForString(dw.RevA),
                                SwapNullForString(dw.RevB),
                                SwapNullForString(dw.RevC),
                                SwapNullForString(dw.RevD),
                                SwapNullForString(dw.RevE),
                                SwapNullForString(dw.RevF),
                                SwapNullForString(dw.RevG),
                                SwapNullForString(dw.Scale),
                                SwapNullForString(dw.Sheet),
                                SwapNullForString(dw.SheetRevision),
                                SwapNullForString(dw.SheetSize),
                                Enum.GetName(typeof(DrawingStatus), dw.Status),
                                SwapNullForString(dw.TitleLine1),
                                SwapNullForString(dw.TitleLine2),
                                SwapNullForString(dw.TitleLine3),
                                SwapNullForString(dw.TitleLine4)
                            }));
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        string SwapNullForString(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return "";
            }
            return str;
        }

        bool CanExportSearchExecute()
        {
            return true;
        }

        public ICommand ExportSearch { get { return new RelayCommand(ExportSearchExecute, CanExportSearchExecute); } }


        void DrawingDoubleClickExecute()
        {
            OpenDrawingExecute();
        }

        bool CanDrawingDoubleClickExecute()
        {
            return true;
        }

        public ICommand DrawingDoubleClick { get { return new RelayCommand(DrawingDoubleClickExecute, CanDrawingDoubleClickExecute); } }

        void ViewDrawingExecute()
        {
            if (SelectedDrawing == null) return;
            ViewDrawingWindow win = new ViewDrawingWindow(SelectedDrawing, RefreshDrawings);
            win.Show();
        }

        bool CanViewDrawingExecute()
        {
            return true;
        }

        public ICommand ViewDrawing { get { return new RelayCommand(ViewDrawingExecute, CanViewDrawingExecute); } }

        void RefreshDrawingDeletionStatusExecute()
        {
            OnPropertyChanged("CanDeleteDrawing");
        }

        bool CanRefreshDrawingDeletionStatusExecute()
        {
            return true;
        }

        public ICommand RefreshDrawingDeletionStatus { get { return new RelayCommand(RefreshDrawingDeletionStatusExecute, CanRefreshDrawingDeletionStatusExecute); } }
        
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
