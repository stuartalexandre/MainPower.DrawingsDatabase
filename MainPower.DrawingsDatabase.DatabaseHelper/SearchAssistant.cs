using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using MPDrawing = MainPower.DrawingsDatabase.DatabaseHelper.Drawing;
using MainPower.DrawingsDatabase.DatabaseHelper;
using System.Reflection;

namespace MainPower.DrawingsDatabase.GUI
{
    /// <summary>
    /// Search options which specifiy to search the
    /// target
    /// </summary>
    public enum TextSearchOptions
    {
        /// <summary>
        /// Search for the exact phrase
        /// </summary>
        SeachExact,
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
    public static class SearchAssisstant
    {
        /// <summary>
        /// Returns the property name of a given selector
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static string GPN<TValue>(Expression<Func<MPDrawing, TValue>> selector)
        {
            return GetProperty(selector).Name;
        }

        /// <summary>
        /// Returns an expression of the kind 'where property.Contains(value)'
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<Func<MPDrawing, bool>> WhereLike(string property, string value)
        {
            var param = Expression.Parameter(typeof(MPDrawing), "x");
            var propertyToSearch = Expression.PropertyOrField(param, property);//memberexpression
            var searchText = Expression.Constant(value, typeof(string));//constantexpression
            var methodInfo = typeof(string).GetMethod("Contains");//methodinfo
            var body = Expression.Call(propertyToSearch, methodInfo, searchText);
            Expression<Func<MPDrawing, bool>> lambda = (Expression<Func<MPDrawing, bool>>)Expression.Lambda<Func<MPDrawing, bool>>(body, param);
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
        public static Expression<Func<MPDrawing, bool>> SearchText(string searchPhrase, TextSearchOptions opt, params Expression<Func<MPDrawing, string>>[] fieldsToSearch)
        {
            int numFields = fieldsToSearch.Count();
            string[] propNames = new string[numFields];
            Expression<Func<MPDrawing, bool>> whereClause = null;
            Expression<Func<MPDrawing, bool>> whereSubClause = null;
            //fetch the property names
            for (int i = 0; i < numFields; i++)
                propNames[i] = GPN(fieldsToSearch[i]);

            //deal with the SearchExact case
            if (opt == TextSearchOptions.SeachExact)
            {
                whereClause = WhereLike(propNames[0], searchPhrase);
                for (int j = 1; j < numFields; j++)
                    whereClause = whereClause.OrElse(WhereLike(propNames[j], searchPhrase));
                return whereClause;
            }
            //tokenise the search phrase
            string[] tokens = searchPhrase.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //if there aren't any tokens (eg a string of spaces), just search the exact phrase instead
            if (tokens.Count() == 0)
                return SearchText(searchPhrase, TextSearchOptions.SeachExact, fieldsToSearch); ;
            
            //create the clause for the first property
            whereClause = WhereLike(propNames[0], tokens[0]);
            for (int i = 1; i < tokens.Length; i++)
                whereClause = opt == TextSearchOptions.SearchAllWords ? whereClause.AndAlso(WhereLike(propNames[0], tokens[i])) : whereClause.OrElse(WhereLike(propNames[0], tokens[i]));
            //if there are more fields, or them all up
            for (int j = 1; j < numFields; j++)
            {
                whereSubClause = WhereLike(propNames[j], tokens[0]);
                for (int i = 1; i < tokens.Length; i++)
                    whereSubClause = opt == TextSearchOptions.SearchAllWords ? whereSubClause.AndAlso(WhereLike(propNames[j], tokens[i])) : whereSubClause.OrElse(WhereLike(propNames[j], tokens[i]));
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
        public static PropertyInfo GetProperty<T, TValue>(Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (PropertyInfo)((MemberExpression)body).Member;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
     
}