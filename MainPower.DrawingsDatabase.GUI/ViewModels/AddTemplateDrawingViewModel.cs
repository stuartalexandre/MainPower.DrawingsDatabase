using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MainPower.DrawingsDatabase.Gui.Models;
using MicroMvvm;
using Microsoft.Win32;

namespace MainPower.DrawingsDatabase.Gui.ViewModels
{
    public class AddTemplateDrawingViewModel : ViewModelBase
    {
        private ObservableCollection<TemplateDrawingModel> _recentTemplates;
        private TemplateDrawingModel _selectedTemplate;
        private string _statusMessage;

        #region Properties
        
        /// <summary>
        /// The currently selected template
        /// </summary>
        public TemplateDrawingModel SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (SelectedTemplate == value) return;
                _selectedTemplate = value;
                OnPropertyChanged("SelectedTemplate");
            }
        }

        /// <summary>
        /// The currently selected template
        /// </summary>
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (StatusMessage == value) return;
                _statusMessage = DateTime.Now.ToShortTimeString() + ": " + value;
                OnPropertyChanged("StatusMessage");
            }
        }

        /// <summary>
        /// Recently used templates
        /// </summary>
        public ObservableCollection<TemplateDrawingModel> RecentTemplates
        {
            get { return _recentTemplates; }
            set
            {
                if (RecentTemplates == value) return;
                _recentTemplates = value;
                OnPropertyChanged("RecentTemplates");
            }
        }
        #endregion

        /// <summary>
        /// Load the list of recently used templates, and add the default template
        /// </summary>
        public AddTemplateDrawingViewModel()
        {
            //load the default template
            Drawing d = DBCommon.CreateDefaultDrawing();
            _recentTemplates = new ObservableCollection<TemplateDrawingModel>();
            _recentTemplates.Add(new TemplateDrawingModel("(default)", d));

            //read the recent templates, and attempt to load them
            string[] templates = Properties.Settings.Default.RecentTemplates.Split(new char[] { ';' });
            foreach (string file in templates)
            {
                try
                {
                    if (!File.Exists(file))
                    {
                        continue;
                    }
                    _recentTemplates.Add(new TemplateDrawingModel(file));
                }
                //if we have a problem then just ignore it and continue
                //TODO: perhaps log these errors somewhere
                catch { }
            }
        }

        #region Commands

        /// <summary>
        /// TODO: should we save the templates themselves here too?
        /// </summary>
        private void ClosingExecute()
        {
            string templates = "";
            foreach (TemplateDrawingModel o in _recentTemplates)
            {
                //ignore the default template, because it is not persisted
                if (o.Path == "(default)")
                    continue;
                templates += o.Path + ";";
            }
            Properties.Settings.Default.RecentTemplates = templates;
            Properties.Settings.Default.Save();
        }

        private bool CanClosingExecute()
        {
            return true;
        }

        /// <summary>
        /// To be executed when the view is closing, so we can save the list of recently used templates
        /// </summary>
        public ICommand Closing { get { return new RelayCommand(ClosingExecute, CanClosingExecute); } }

        private void AddExistingTemplateExecute()
        {
            try
            {
                OpenFileDialog d = new OpenFileDialog();
                if (d.ShowDialog() == true)
                {
                    _recentTemplates.Add(new TemplateDrawingModel(d.FileName));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool CanAddExistingTemplateExecute()
        {
            return true;
        }

        /// <summary>
        /// Prompt to open an existing template and add to the MRU list
        /// </summary>
        public ICommand AddExistingTemplate { get { return new RelayCommand(AddExistingTemplateExecute, CanAddExistingTemplateExecute); } }

        private void AddDrawingExecute()
        {
            try
            {
                if (SelectedTemplate != null)
                {
                    SelectedTemplate.AddDrawingToDatabase();
                    StatusMessage = "Added drawing " + SelectedTemplate.Drawing.Number + " to the database.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool CanAddDrawingExecute()
        {
            return true;
        }

        /// <summary>
        /// Add the drawing from the currently selected template to the database
        /// </summary>
        public ICommand AddDrawing { get { return new RelayCommand(AddDrawingExecute, CanAddDrawingExecute); } }

        private void DeleteTemplateExecute()
        {
            try
            {
                _recentTemplates.Remove(SelectedTemplate);
            }
            catch { }
        }

        private bool CanDeleteTemplateExecute()
        {
            return true;
        }

        /// <summary>
        /// Remove a template from the list.  Does not delete the template file if it exists.
        /// </summary>
        public ICommand DeleteTemplate { get { return new RelayCommand(DeleteTemplateExecute, CanDeleteTemplateExecute); } }

        private void AddTemplateExecute()
        {
            Drawing d = DBCommon.CreateDefaultDrawing();
            d.Number = "";
            _recentTemplates.Add(new TemplateDrawingModel("[new template]", d));
        }

        private bool CanAddTemplateExecute()
        {
            return true;
        }

        /// <summary>
        /// Add a new template
        /// </summary>
        public ICommand AddTemplate { get { return new RelayCommand(AddTemplateExecute, CanAddTemplateExecute); } }

        private void SaveTemplateExecute()
        {
            if (SelectedTemplate != null)
            {
                SelectedTemplate.SaveTemplate();
            }
        }

        private bool CanSaveTemplateExecute()
        {
            return true;
        }

        /// <summary>
        /// Save the selected template.  If the path does not exist then user will be prompted for a new path.
        /// </summary>
        public ICommand SaveTemplate { get { return new RelayCommand(SaveTemplateExecute, CanSaveTemplateExecute); } }

        private void SaveTemplateAsExecute()
        {
            if (SelectedTemplate != null)
            {
                var newTemplate = SelectedTemplate.SaveTemplateAs();
                if (newTemplate != null)
                {
                    _recentTemplates.Add(newTemplate);
                }
            }
        }

        private bool CanSaveTemplateAsExecute()
        {
            return true;
        }

        /// <summary>
        /// Displays a SavefileDialog and saves the template
        /// </summary>
        public ICommand SaveTemplateAs { get { return new RelayCommand(SaveTemplateAsExecute, CanSaveTemplateAsExecute); } }

        private void ResetTemplateExecute()
        {
            if (SelectedTemplate != null)
            {
                SelectedTemplate.ResetTemplate();
            }
        }

        private bool CanResetTemplateExecute()
        {
            return true;
        }

        /// <summary>
        /// Resets the current state of the template to its original state
        /// </summary>
        public ICommand ResetTemplate { get { return new RelayCommand(ResetTemplateExecute, CanResetTemplateExecute); } }

        #endregion
    }
}
