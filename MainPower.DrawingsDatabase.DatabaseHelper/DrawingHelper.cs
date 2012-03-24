using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    /// <summary>
    /// Contains extension methods and helper functions for the Drawing class
    /// </summary>
    public static class DrawingHelper
    {
        /// <summary>
        /// Maps AutoCAD attributes to Drawing Properties
        /// Unfortunately there are a few different templates floating about, 
        /// so lets map the various attribute names to a single database column
        /// </summary>
        private static readonly Dictionary<string, string> AttribMap = new Dictionary<string, string>
                                                                           {
                                                                               {"PROJECT_TITLE", "ProjectTitle"},
                                                                               {"TITLE_LINE_1", "TitleLine1"},
                                                                               {"TITLE_LINE_2", "TitleLine2"},
                                                                               {"TITLE_LINE_3", "TitleLine3"},
                                                                               {"DRAWING-NUMBER", "Number"},
                                                                               {"DRAWING CATEGORY", "Category"},
                                                                               {"DRAWING-CATEGORY", "Category"},
                                                                               {"DWG-STATUS", "Status"},
                                                                               {"SHEET_REVISION:", "SheetRevision"},
                                                                               {"SHEET", "Sheet"},
                                                                               {"SCALE", "Scale"},
                                                                               {"DRAWN_BY", "DrawnBy"},
                                                                               {"DRAWN-DATE", "DrawnDate"},
                                                                               {"CHECKED_BY", "CheckedBy"},
                                                                               {"CHCKD-DATE", "CheckedDate"},
                                                                               {"APPROVED_BY", "ApprovedBy"},
                                                                               {"APPD-DATE", "ApprovedDate"},
                                                                               {"REV-A", "RevA"},
                                                                               {"REV-B", "RevB"},
                                                                               {"REV-C", "RevC"},
                                                                               {"REV-D", "RevD"},
                                                                               {"REV-E", "RevE"},
                                                                               {"REV-F", "RevF"},
                                                                               {"REV-G", "RevG"},
                                                                               {"BY-A", "DrawnByRevA"},
                                                                               {"BY-B", "DrawnByRevB"},
                                                                               {"BY-C", "DrawnByRevC"},
                                                                               {"BY-D", "DrawnByRevD"},
                                                                               {"BY-E", "DrawnByRevE"},
                                                                               {"BY-F", "DrawnByRevF"},
                                                                               {"BY-G", "DrawnByRevG"},
                                                                               {"DATE-A", "DateRevA"},
                                                                               {"DATE-B", "DateRevB"},
                                                                               {"DATE-C", "DateRevC"},
                                                                               {"DATE-D", "DateRevD"},
                                                                               {"DATE-E", "DateRevE"},
                                                                               {"DATE-F", "DateRevF"},
                                                                               {"DATE-G", "DateRevG"},
                                                                               {"DESCRIPTION-A", "DescriptionRevA"},
                                                                               {"DESCRIPTION-B", "DescriptionRevB"},
                                                                               {"DESCRIPTION-C", "DescriptionRevC"},
                                                                               {"DESCRIPTION-D", "DescriptionRevD"},
                                                                               {"DESCRIPTION-E", "DescriptionRevE"},
                                                                               {"DESCRIPTION-F", "DescriptionRevF"},
                                                                               {"DESCRIPTION-G", "DescriptionRevG"},
                                                                               {"APPRV-BY-A", "ApprovedByRevA"},
                                                                               {"APPRV-BY-B", "ApprovedByRevB"},
                                                                               {"APPRV-BY-C", "ApprovedByRevC"},
                                                                               {"APPRV-BY-D", "ApprovedByRevD"},
                                                                               {"APPRV-BY-E", "ApprovedByRevE"},
                                                                               {"APPRV-BY-F", "ApprovedByRevF"},
                                                                               {"APPRV-BY-G", "ApprovedByRevG"}
                                                                           };

        /// <summary>
        /// Maps AutoCAD attribute names to Drawing object properties
        /// </summary>
        private static readonly Dictionary<string, PropertyInfo> Attribs = new Dictionary<string, PropertyInfo>();

        static DrawingHelper()
        {
            Type drawingType = typeof (Drawing);
            foreach (var kvp in AttribMap)
            {
                Attribs.Add(kvp.Key, drawingType.GetProperty(kvp.Value));
            }
        }

        /// <summary>
        /// Set a property based on the AutoCAD attribute name
        /// </summary>
        /// <param name="drawing"> </param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public static void SetAttribute(this Drawing drawing, string attribute, string value)
        {
            if (Attribs.ContainsKey(attribute))
            {
                Type t = Attribs[attribute].PropertyType;

                object obj;

                if (t == typeof (string))
                    obj = value;
                else if (t == typeof (DrawingCategory))
                    obj = new DrawingCategoryConverter().ConvertBack(value, typeof (DrawingCategory), null, null);
                else if (t == typeof (DrawingStatus))
                    obj = new DrawingStatusConverter().ConvertBack(value, typeof (DrawingStatus), null, null);
                else if (t == typeof (DateTime?))
                {
                    DateTime time;

                    if (DateTime.TryParse(value, out time))
                    {
                        obj = time;
                    }
                    else
                    {
                        obj = null;
                    }
                }
                else
                    obj = value;

                Attribs[attribute].SetValue(drawing, obj, null);
            }
            else
            {
                throw new ArgumentException("No such property!");
            }
        }

        /// <summary>
        /// Get a Property based on the AutoCAD attribute name 
        /// </summary>
        /// <param name="drawing"> </param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetAttribute(this Drawing drawing, string attribute)
        {
            if (Attribs.ContainsKey(attribute))
            {
                Type t = Attribs[attribute].PropertyType;
                object obj = Attribs[attribute].GetValue(drawing, null);
                if (obj == null) return "";
                if (t == typeof (string))
                    return (string) obj;
                if (t == typeof (DrawingCategory))
                    return
                        ((string)
                         new DrawingCategoryConverter().Convert((DrawingCategory) obj, typeof (string), null, null)).
                            ToUpper(CultureInfo.InvariantCulture);
                if (t == typeof (DrawingStatus))
                    return
                        ((string) new DrawingStatusConverter().Convert((DrawingStatus) obj, typeof (string), null, null))
                            .ToUpper(CultureInfo.InvariantCulture);
                if (t == typeof (DateTime?))
                {
                    if (obj as DateTime? != null)
                        return ((DateTime) obj).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    return "";
                }
                return obj.ToString();
            }
            return "";
            //TODO throw exception??
        }

        /// <summary>
        /// Determines if the particular attribute is mapped to a Drawing property
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        public static bool AttributeExists(string att)
        {
            return Attribs.ContainsKey(att);
        }
    }

    public partial class Drawing
    {
        public override string ToString()
        {
            return Number + ": " + TitleLine1;
        }
    }
}