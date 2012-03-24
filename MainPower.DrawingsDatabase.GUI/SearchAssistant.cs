using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MainPower.DrawingsDatabase.DatabaseHelper;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;

namespace MainPower.DrawingsDatabase.Gui
{
    /// <summary>
    /// Search options which specifiy to search the
    /// target
    /// </summary>
    public enum TextSearchOption
    {
        /// <summary>
        /// Search for the exact phrase
        /// </summary>
        SearchExact,

        /// <summary>
        /// Search for all the words in the search phrase, in any order
        /// </summary>
        SearchAllWords,

        /// <summary>
        /// Search for any of the words in the search phrase
        /// </summary>
        SearchAnyWords
    }

    /// <summary>
    /// Helper class to assist with searching text
    /// </summary>
    public static class SearchAssistant
    {
        /// <summary>
        /// Returns the property name of a given selector
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        private static string GPN<TValue>(Expression<Func<MPDrawing, TValue>> selector)
        {
            return GetProperty(selector).Name;
        }

        /// <summary>
        /// Returns an expression of the kind 'where property.Contains(value)'
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression<Func<MPDrawing, bool>> WhereLike(string property, string value)
        {
            ParameterExpression param = Expression.Parameter(typeof (MPDrawing), "x");
            MemberExpression propertyToSearch = Expression.PropertyOrField(param, property); //memberexpression
            ConstantExpression searchText = Expression.Constant(value, typeof (string)); //constantexpression
            MethodInfo methodInfo = typeof (string).GetMethod("Contains"); //methodinfo
            MethodCallExpression body = Expression.Call(propertyToSearch, methodInfo, searchText);
            Expression<Func<MPDrawing, bool>> lambda = Expression.Lambda<Func<MPDrawing, bool>>(body, param);
            return lambda;
        }

        /// <summary>
        /// Builds a lambda expression that can be used in a where clause, 
        /// that searches for the given phrase (considering the search 
        /// options) in the given property name(s)
        /// 
        /// Eg to search for the exact phrase "This is a test phrase" in the
        /// field/property "TestField" of some object, you could call
        /// var res = someCollection.Where(SearchText("This is a test phrase", TextSearchOptions.SearchExact, o=>o.TestField));
        /// 
        /// It is important to note that the properties are OR'd together, 
        /// ie the returned expression will return true if the searchPhrase
        /// is found in ANY of the properties.
        /// </summary>
        /// <param name="searchPhrase"></param>
        /// <param name="opt"></param>
        /// <param name="fieldsToSearch"></param>
        /// <returns></returns>
        public static Expression<Func<MPDrawing, bool>> SearchText(string searchPhrase, TextSearchOption opt,
                                                                   params Expression<Func<MPDrawing, string>>[]
                                                                       fieldsToSearch)
        {
            int numFields = fieldsToSearch.Count();
            var propNames = new string[numFields];
            Expression<Func<MPDrawing, bool>> whereClause;
            //fetch the property names
            for (int i = 0; i < numFields; i++)
                propNames[i] = GPN(fieldsToSearch[i]);

            //deal with the SearchExact case
            if (opt == TextSearchOption.SearchExact)
            {
                whereClause = WhereLike(propNames[0], searchPhrase);
                for (int j = 1; j < numFields; j++)
                    whereClause = whereClause.OrElse(WhereLike(propNames[j], searchPhrase));
                return whereClause;
            }
            //tokenise the search phrase
            string[] tokens = searchPhrase.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            //if there aren't any tokens (eg a string of spaces), just search the exact phrase instead
            if (!tokens.Any())
                return SearchText(searchPhrase, TextSearchOption.SearchExact, fieldsToSearch);

            //create the clause for the first property
            whereClause = WhereLike(propNames[0], tokens[0]);
            for (int i = 1; i < tokens.Length; i++)
                whereClause = opt == TextSearchOption.SearchAllWords
                                  ? whereClause.AndAlso(WhereLike(propNames[0], tokens[i]))
                                  : whereClause.OrElse(WhereLike(propNames[0], tokens[i]));
            //if there are more fields, or them all up
            for (int j = 1; j < numFields; j++)
            {
                Expression<Func<MPDrawing, bool>> whereSubClause = WhereLike(propNames[j], tokens[0]);
                for (int i = 1; i < tokens.Length; i++)
                    whereSubClause = opt == TextSearchOption.SearchAllWords
                                         ? whereSubClause.AndAlso(WhereLike(propNames[j], tokens[i]))
                                         : whereSubClause.OrElse(WhereLike(propNames[j], tokens[i]));
                whereClause = whereClause.OrElse(whereSubClause);
            }
            return whereClause;
        }

        /// <summary>
        /// Returns the poperty info object of a property, when passed as an argument
        /// in the form o => o.Property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        private static PropertyInfo GetProperty<T, TValue>(Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression) body).Body;
            }
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (PropertyInfo) ((MemberExpression) body).Member;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}