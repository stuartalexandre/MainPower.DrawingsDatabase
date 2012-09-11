﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MainPower.DrawingsDatabase.Gui.ViewModels;
using Microsoft.Win32;

namespace MainPower.DrawingsDatabase.Gui.Models
{
    /// <summary>
    /// TODO: This class it a model, which inherits from ViewModelBase.  Which is it?? probably a viewmodel I thinks.
    /// </summary>
    public class TemplateDrawingModel : ViewModelBase
    {
        private Drawing _drawing;
        private Drawing _originalDrawing;
        private string _path;

        #region Properties

        /// <summary>
        /// The filename of the drawing template
        /// </summary>
        public string FileName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(_path); }
        }

        /// <summary>
        /// The full path of the template
        /// </summary>
        public string Path
        {
            get { return _path; }
            private set
            {
                if (Path == value) return;
                _path = value;
                OnPropertyChanged("Path");
            }
        }
        
        /// <summary>
        /// The current state of the drawing template
        /// </summary>
        public Drawing Drawing
        {
            get { return _drawing; }
            private set
            {
                if (Drawing == value) return;
                _drawing = value;
                OnPropertyChanged("Drawing");
            }
        }

        /// <summary>
        /// The original state of the drawing template
        /// </summary>
        public Drawing OriginalDrawing
        {
            get { return _originalDrawing; }
            private set
            {
                if (OriginalDrawing == value) return;
                _originalDrawing = value;
                OnPropertyChanged("OriginalDrawing");
            }
        }

        #endregion

        /// <summary>
        /// Create a new drawing template from the persisted/serialized drawing
        /// </summary>
        /// <param name="path"></param>
        public TemplateDrawingModel(string path) 
        {
            StreamReader sr = new StreamReader(path);
            XmlSerializer xml = new XmlSerializer(typeof(Drawing));
            Drawing d = xml.Deserialize(sr) as Drawing;
            if (d != null)
            {
                _path = path;
                _originalDrawing = d;
                Drawing = _originalDrawing.Clone() as Drawing;
            }
        }

        /// <summary>
        /// Create a drawing template from given drawing with the given path/title
        /// </summary>
        /// <param name="title"></param>
        /// <param name="d"></param>
        public TemplateDrawingModel(string title, Drawing d)
        {
            _path = title;
            //clone the drawing so subsequent changes effect only one copy
            _originalDrawing = d.Clone() as Drawing;
            //same here, clone the working copy.  Shallow copy ok as all members are value or immutable reference types.
            Drawing = _originalDrawing.Clone() as Drawing;
        }

        /// <summary>
        /// Return the filename of the template
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FileName;
        }

        /// <summary>
        /// Reset the working copy to the original drawing template
        /// </summary>
        public void ResetTemplate()
        {
            Drawing = _originalDrawing.Clone() as Drawing;
        }

        /// <summary>
        /// Save the template.  If it doesnt already exist, then SaveFileDialog will be displayed.
        /// </summary>
        public void SaveTemplate()
        {
            if (!File.Exists(_path))
            {
                SaveTemplateAs();
                return;
            }
            StreamWriter sw = new StreamWriter(Path);
            XmlSerializer xml = new XmlSerializer(typeof(Drawing));
            xml.Serialize(sw, Drawing);
            OriginalDrawing = Drawing.Clone() as Drawing;
        }

        /// <summary>
        /// Show SaveFileDialog and save template
        /// </summary>
        public void SaveTemplateAs()
        {
            SaveFileDialog d = new SaveFileDialog();
            if (d.ShowDialog() == true)
            {
                Path = d.FileName;
                StreamWriter sw = new StreamWriter(Path);
                XmlSerializer xml = new XmlSerializer(typeof(Drawing));
                xml.Serialize(sw, Drawing);
                OriginalDrawing = Drawing.Clone() as Drawing;
            }
        }

        /// <summary>
        /// Add the working copy drawing to the database
        /// </summary>
        public void AddDrawingToDatabase()
        {
            DrawingsDataContext dc = DBCommon.NewDC;
            dc.Drawings.InsertOnSubmit(Drawing);
            dc.SubmitChanges();
            //clone the working copy, so we dont change an existing drawing
            Drawing = Drawing.Clone() as Drawing;
        }
    }
}