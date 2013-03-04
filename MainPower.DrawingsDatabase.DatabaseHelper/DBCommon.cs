using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using MainPower.DrawingsDatabase.DatabaseHelper.Properties;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Globalization;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    /// <summary>
    /// Contains methods for communicating with the drawings database.
    /// </summary>
    public  static class DBCommon
    {
       public static DrawingsDataContext NewDC { get { return new DrawingsDataContext(); } }

        /// <summary>
        /// Gets the next free drawing number
        /// </summary>
        /// <returns></returns>
       public static string GetNextDrawingNumber()
       {
           int nextNum = 0;

           DrawingsDataContext dc = NewDC;
           //couldnt really work out how to do this with linq, this is nicer anyways
           var dwgs = dc.ExecuteQuery<Drawing>("select top 1 * from Drawings where (CONSULTANT='' OR CONSULTANT IS NULL) AND ISNUMERIC(NUMBER) != 0 order by  CAST(NUMBER AS INT) desc");

           try
           {
               Drawing d2 = dwgs.First();
               nextNum = int.Parse(d2.Number, CultureInfo.InvariantCulture) + 1;
           }
           catch
           {
               nextNum++;
           }

           if (nextNum < 4000) nextNum = 4000; //start series at 4000
           return nextNum.ToString(CultureInfo.InvariantCulture);
       }

        public static void DeleteDrawing(int id)
        {
            DrawingsDataContext dc = NewDC;
            Drawing drawing = (from d in dc.Drawings where d.Id == id select d).Single();
            dc.Drawings.DeleteOnSubmit(drawing);
            dc.SubmitChanges();
        }

        #region LINQ AndAlso/OrElse Extension Methods

        public static Func<T, bool> AndAlso<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            return arg => predicate1(arg) && predicate2(arg);
        }

        public static Func<T, bool> OrElse<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            return arg => predicate1(arg) || predicate2(arg);
        }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.AndAlso(
                    Expression.Invoke(left, param),
                    Expression.Invoke(right, param)
                );
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return lambda;
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.OrElse(
                    Expression.Invoke(left, param),
                    Expression.Invoke(right, param)
                );
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return lambda;
        }

        #endregion
        
        /// <summary>
        /// Determine if a drawing number/sheet number combo exists (for new drawings)
        /// </summary>
        /// <param name="number"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static bool DrawingComboExists(string number, string sheet)
        {
            if (number == null || sheet == null)
                return true;
            var res = from d in NewDC.Drawings where d.Number == number && d.Sheet == sheet select d;
            if (res.Count() == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Detemine if a drawing number/sheet number combo exists (for existing drawings)
        /// </summary>
        /// <param name="number"></param>
        /// <param name="sheet"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DrawingComboExists(string number, string sheet, int id)
        {
            if (number == null || sheet == null)
                return true;
            var res = from d in NewDC.Drawings where d.Number == number && d.Sheet == sheet && d.Id != id select d;
            if (res.Count() == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Creates a new drawing object, with the next available drawing number
        /// and sheet=1, with some arbitrary prompt text in the project and titleline1 fields
        /// </summary>
        /// <returns></returns>
        public static Drawing CreateDefaultDrawing()
        {
            Drawing d = new Drawing();
            d.DrawnDate = DateTime.Now;
            d.Number = GetNextDrawingNumber();
            d.Sheet = "1";
            d.ProjectTitle = "YOUR PROJECT TITLE HERE";
            d.TitleLine1 = "YOUR TITLE LINE 1 HERE";
            d.Electronic = true;
            return d;
        }

        public static void OpenAutoCadDrawing(string fileName)
        {
            //string launcherPath = Settings.Default.AcLauncherPath;
            //if (!File.Exists(launcherPath)) throw new FileNotFoundException("Cannot find autocad launcher!");
            if (string.IsNullOrEmpty(fileName))
                throw new FileNotFoundException("There is no file associated with this drawing.");
            if (!File.Exists(fileName)) 
                throw new FileNotFoundException("Cannot find the drawing: " + fileName);

            //Process.Start(launcherPath, "/O + \"" + fileName + "\"");
            Process.Start(fileName);
        }

        public static void InstallPlugin(AcadVersion version)
        {
            string rootPath = "";

            switch (version)
            {
                case AcadVersion.ACAD2010:

                    rootPath = "HKEY_CURRENT_USER\\Software\\Autodesk\\AutoCAD\\R18.0\\ACAD-8001:409\\Applications\\DrawingsDB";
                    break;
                case AcadVersion.ACAD2012:
                    rootPath = "HKEY_CURRENT_USER\\Software\\Autodesk\\AutoCAD\\R18.2\\ACAD-A001:409\\Applications\\DrawingsDB";
                    break;
                default:
                    throw new ArgumentException("Version not supported");
            }
            //DESCRIPTION
            string description = "Drawings database helper";
            //LOADER
            string loader = AppDomain.CurrentDomain.BaseDirectory + "AutoCADPlugin.dll";
            //LOADCTRLS
            int loadCtrls = 2;
            //MANAGED
            int managed = 1;


            Microsoft.Win32.Registry.SetValue(rootPath, "DESCRIPTION", description);
            Microsoft.Win32.Registry.SetValue(rootPath, "LOADER", loader);
            Microsoft.Win32.Registry.SetValue(rootPath, "LOADCTRLS", loadCtrls, Microsoft.Win32.RegistryValueKind.DWord);
            Microsoft.Win32.Registry.SetValue(rootPath, "MANAGED", managed, Microsoft.Win32.RegistryValueKind.DWord);
        }

        public static void RemovePlugin(AcadVersion version)
        {
            string rootPath = "";

            switch (version)
            {
                case AcadVersion.ACAD2010:

                    rootPath = "HKEY_CURRENT_USER\\Software\\Autodesk\\AutoCAD\\R18.0\\ACAD-8001:409\\Applications\\DrawingsDB";
                    break;
                case AcadVersion.ACAD2012:
                    rootPath = "HKEY_CURRENT_USER\\Software\\Autodesk\\AutoCAD\\R18.2\\ACAD-A001:409\\Applications\\DrawingsDB";
                    break;
                default:
                    throw new ArgumentException("Version not supported");
            }
            Microsoft.Win32.Registry.CurrentUser.DeleteSubKey(rootPath);
        }

        
    }
}
