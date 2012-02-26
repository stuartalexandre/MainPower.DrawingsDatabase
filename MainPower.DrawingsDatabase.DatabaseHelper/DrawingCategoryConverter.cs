﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Globalization;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    /// <summary>
    /// Converts a DrawingCategory Enum to a 'nice' string
    /// </summary>
    public class DrawingCategoryConverter : IValueConverter
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
            if (value is DrawingCategory && targetType == typeof(string))
            {
                DrawingCategory cat = (DrawingCategory)value;

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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DrawingCategory.Undefined;
            string str = value as string;
            if (str == null)
                return null;
            switch (str.ToUpperInvariant())
            {
                case "COMMUNICATIONS":
                    return DrawingCategory.Communications;
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
    }
}