using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Globalization;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    /// <summary>
    /// Converts a Date Enum to a 'nice' string
    /// </summary>
    public class DateConverter : IValueConverter
    {
        /// <summary>
        /// Convert DrawingCategory to string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime && targetType == typeof(string))
            {
                return ((DateTime)value).ToShortDateString();
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DateTime.Parse((string)value, CultureInfo.InvariantCulture);
        }
    }
}
