using System;
using System.Globalization;
using System.Windows.Data;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    /// <summary>
    /// Converts a DrawingCategory Enum to a 'nice' string
    /// </summary>
    public class DrawingCategoryConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Convert DrawingCategory to string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DrawingCategory && targetType == typeof (string))
            {
                var cat = (DrawingCategory) value;

                switch (cat)
                {
                    case DrawingCategory.Communications:
                        return "Communications";
                    case DrawingCategory.GxpSubstation:
                        return "GXP Substation";
                    case DrawingCategory.Miscellaneous:
                        return "Miscellaneous";
                    case DrawingCategory.Overhead:
                        return "Overhead";
                    case DrawingCategory.Underground:
                        return "Underground";
                    case DrawingCategory.ZoneSubstation:
                        return "Zone Substation";
                    case DrawingCategory.Undefined:
                        return "Undefined";
                    case DrawingCategory.Generation:
                        return "Generation";
                }
            }
            return "Undefined";
        }

        /// <summary>
        /// Convert string to DrawingCategory
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DrawingCategory.Undefined;
            var str = value as string;
            if (str == null)
                return null;
            switch (str.ToUpperInvariant())
            {
                case "COMMUNICATIONS":
                    return DrawingCategory.Communications;
                case "GENERATION":
                    return DrawingCategory.Generation;
                case "GXP SUBSTATION":
                    return DrawingCategory.GxpSubstation;
                case "MISCELLANEOUS":
                    return DrawingCategory.Miscellaneous;
                case "OVERHEAD":
                    return DrawingCategory.Overhead;
                case "UNDERGROUND":
                    return DrawingCategory.Underground;
                case "ZONE SUBSTATION":
                    return DrawingCategory.ZoneSubstation;
                default:
                    return DrawingCategory.Undefined;
            }
        }

        #endregion
    }
}