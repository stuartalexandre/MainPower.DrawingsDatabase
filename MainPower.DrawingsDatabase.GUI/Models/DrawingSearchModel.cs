﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace MainPower.DrawingsDatabase.Gui.Models
{
    public class DrawingSearchModel
    {
        public string DrawingNumber
        {
            get;
            set;
        }
        public string Title1
        {
            get;
            set;
        }
        public string Title2
        {
            get;
            set;
        }
        public string Title3
        {
            get;
            set;
        }
        public string ProjectTitle
        {
            get;
            set;
        }
        public DateTime StartDate
        {
            get;
            set;
        }
        public DateTime EndDate
        {
            get;
            set;
        }
        public TextSearchOption SearchType
        {
            get;
            set;
        }
        public bool ElectronicOnly
        {
            get;
            set;
        }
        public bool NonElectronicOnly
        {
            get;
            set;
        }
        public bool StorageAgnostic
        {
            get;
            set;
        }
        public bool SearchAllTitles
        {
            get;
            set;
        }
        public bool IncludeLegacyNumbers
        {
            get;
            set;
        }
        public bool SearchDateRange
        {
            get;
            set;
        }
        public bool CategoryAll
        {
            get;
            set;
        }
        public bool CategoryMiscellaneous
        {
            get;
            set;
        }
        public bool CategoryOverhead
        {
            get;
            set;
        }
        public bool CategoryGXPSubstation
        {
            get;
            set;
        }
        public bool CategoryZoneSubstation
        {
            get;
            set;
        }
        public bool CategoryUnderground
        {
            get;
            set;
        }
        public bool CategoryCommunications
        {
            get;
            set;
        }
        public bool CategoryGeneration
        {
            get;
            set;
        }
        public bool StatusAll
        {
            get;
            set;
        }
        public bool StatusAsBuilt
        {
            get;
            set;
        }
        public bool StatusDraft
        {
            get;
            set;
        }
        public bool StatusCanceled
        {
            get;
            set;
        }
        public bool StatusSuperseded
        {
            get;
            set;
        }
        public bool StatusPlanned
        {
            get;
            set;
        }
        public bool StatusCurrent
        {
            get;
            set;
        }
        public String AdvancedSearchQuery
        {
            get;
            set;
        }

        public DrawingSearchModel()
        {
            StartDate = DateTime.Parse("1/1/1970");
            EndDate = DateTime.Parse("1/1/2020");
            CategoryAll = true;
            StatusAll = true;
            ElectronicOnly = false;
            StorageAgnostic = true;
            IncludeLegacyNumbers = true;
            NonElectronicOnly = false;
            AdvancedSearchQuery = "Number.Contains(\"13\") OR (Status== \"CURRENT\" AND Category== \"ZONESUBSTATION\")";
            SearchAllTitles = true;
            SearchType = TextSearchOption.SearchAllWords;
        }

        /// <summary>
        /// Deserialise a xml file into a DrawingSearchModel object.
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public static DrawingSearchModel CreateFromXml(string xmlPath)
        {
            StreamReader sr = new StreamReader(xmlPath);
            XmlSerializer xml = new XmlSerializer(typeof(DrawingSearchModel));
            DrawingSearchModel d = xml.Deserialize(sr) as DrawingSearchModel;
            return d;
        }

        /// <summary>
        /// Deserialise a string of xml data into a DrawingSearchModel object.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DrawingSearchModel CreateFromString(string str)
        {
            byte[] data = ASCIIEncoding.ASCII.GetBytes(str);
            MemoryStream ms = new MemoryStream(data);
            XmlSerializer xml = new XmlSerializer(typeof(DrawingSearchModel));
            DrawingSearchModel d = xml.Deserialize(ms) as DrawingSearchModel;
            return d;
        }
    }
}
