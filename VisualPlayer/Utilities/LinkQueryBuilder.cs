using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Utilities
{
    public static class LinkQueryBuilder
    {

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Create Linq query starting with True condition. </summary>
        /// <typeparam name="T"> Type of object. </typeparam>
        /// <returns> Linq Query. </returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Create Linq query starting with False condition. </summary>
        /// <typeparam name="T"> Type of object. </typeparam>
        /// <returns> Linq query. </returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Concatenate Linq query using OR. </summary>
        /// <typeparam name="T"> Type of object. </typeparam>
        /// <param name="expr1"> First query. </param>
        /// <param name="expr2"> Second query. </param>
        /// <returns> Linq query. </returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        //  ----------------------------------------------------------------------------------------------------
        /// <summary> Concatenate Linq query using AND. </summary>
        /// <typeparam name="T"> Type of object. </typeparam>
        /// <param name="expr1"> First query. </param>
        /// <param name="expr2"> Second query. </param>
        /// <returns> Linq query. </returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

    }
}
