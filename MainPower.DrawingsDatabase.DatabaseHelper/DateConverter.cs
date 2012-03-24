using System;
using System.Globalization;
using System.Windows.Data;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    /// <summary>
    /// Converts a Date Enum to a 'nice' string
    /// </summary>
    public class DateConverter : IValueConverter
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
            if (value is DateTime && targetType == typeof (string))
            {
                return ((DateTime) value).ToShortDateString();
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
            return DateTime.Parse((string) value, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}