using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    public enum SheetSize
    {
        A4L,//0
        A4P,//1
        A3L,//2
        A3P,//3
        A2L,//4
        A2P,//5
        A1L,//6
        A1P,//7
        CUSTOM,//8
        UNDEFINED,//9
    }

    /// <summary>
    /// Converts a DrawingCategory Enum to a 'nice' string
    /// </summary>
    public class SheetSizeConverter : IValueConverter
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
            if (value is SheetSize && targetType == typeof(string))
            {
                SheetSize cat = (SheetSize)value;

                
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
            if (value == null)
                return SheetSize.UNDEFINED;
            string str = value as string;
            if (str == null)
                return null;
            switch (str.ToLower())
            {
                default:
                    return SheetSize.UNDEFINED;
            }
        }
    }
}

