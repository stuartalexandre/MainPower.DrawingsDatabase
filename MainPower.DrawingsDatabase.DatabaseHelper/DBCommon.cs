using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    /// <summary>
    /// Contains methods for communicating with the drawings database.
    /// </summary>
    public static class DBCommon
    {
        public static DrawingsDataContext NewDC
        {
            get { return new DrawingsDataContext(); }
        }

        /// <summary>
        /// Gets the next free drawing number
        /// </summary>
        /// <returns></returns>
        public static string GetNextDrawingNumber()
        {
            int nextNum = 0;

            DrawingsDataContext dc = NewDC;
            //couldnt really work out how to do this with linq, this is nicer anyways
            IEnumerable<Drawing> dwgs =
                dc.ExecuteQuery<Drawing>(
                    "select top 1 * from Drawings where (CONSULTANT='' OR CONSULTANT IS NULL) AND ISNUMERIC(NUMBER) != 0 order by  CAST(NUMBER AS INT) desc");

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
            IQueryable<Drawing> res = from d in NewDC.Drawings where d.Number == number && d.Sheet == sheet select d;
            return res.Any();
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
            IQueryable<Drawing> res = from d in NewDC.Drawings
                                      where d.Number == number && d.Sheet == sheet && d.Id != id
                                      select d;
            return res.Any();
        }

        /// <summary>
        /// Creates a new drawing object, with the next available drawing number
        /// and sheet=1, with some arbitrary prompt text in the project and titleline1 fields
        /// </summary>
        /// <returns></returns>
        public static Drawing CreateDefaultDrawing()
        {
            var d = new Drawing();
            d.DrawnDate = DateTime.Now;
            d.Number = GetNextDrawingNumber();
            d.Sheet = "1";
            d.ProjectTitle = "YOUR PROJECT TITLE HERE";
            d.TitleLine1 = "YOUR TITLE LINE 1 HERE";
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

        #region LINQ AndAlso/OrElse Extension Methods

        public static Func<T, bool> AndAlso<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            return arg => predicate1(arg) && predicate2(arg);
        }

        public static Func<T, bool> OrElse<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            return arg => predicate1(arg) || predicate2(arg);
        }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> left,
                                                           Expression<Func<T, bool>> right)
        {
            ParameterExpression param = Expression.Parameter(typeof (T), "x");
            BinaryExpression body = Expression.AndAlso(
                Expression.Invoke(left, param),
                Expression.Invoke(right, param)
                );
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return lambda;
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> left,
                                                          Expression<Func<T, bool>> right)
        {
            ParameterExpression param = Expression.Parameter(typeof (T), "x");
            BinaryExpression body = Expression.OrElse(
                Expression.Invoke(left, param),
                Expression.Invoke(right, param)
                );
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return lambda;
        }

        #endregion
    }
}