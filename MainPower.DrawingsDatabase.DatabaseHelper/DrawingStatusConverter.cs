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
    /// converts a DrawingStatus Enum to a 'nice' string
    /// </summary>
    public class DrawingStatusConverter : IValueConverter
    {
        /// <summary>
        /// Convert DrawingStatus to sring
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DrawingStatus && targetType == typeof(string))
            {
                DrawingStatus status = (DrawingStatus)value;

                switch (status)
                {
                    case DrawingStatus.AsBuilt:
                        return "As Built";
                    case DrawingStatus.Canceled:
                        return "Cancelled";
                    case DrawingStatus.Current:
                        return "Current";
                    case DrawingStatus.Planned:
                        return "Planned";
                    case DrawingStatus.Superseded:
                        return "Superseded";
                    case DrawingStatus.Undefined:
                        return "Undefined";
                }
            }
            return "Undefined";
        }

        /// <summary>
        /// Convert string to DrawingStatus
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DrawingStatus.Undefined;
            string str = value as string;
            if (str == null)
                return null;
            switch (str.ToUpperInvariant())
            {
                case "AS Built":
                    return DrawingStatus.AsBuilt;
                case "CANCELLED":
                    return DrawingStatus.Canceled;
                case "CURRENT":
                    return DrawingStatus.Current;
                case "PLANNED":
                    return DrawingStatus.Planned;
                case "SUPERSEDED":
                    return DrawingStatus.Superseded;
                default:
                    return DrawingStatus.Undefined;
            }
        }
    }
}
