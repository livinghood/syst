using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.General_Logic
{
    public static class Sort
    {
        /// <summary>
        /// Sorts an IQueryable and returns it as an IEnumerable
        /// </summary>
        /// <typeparam name="T">Independent type</typeparam>
        /// <param name="source">The query to sort</param>
        /// <param name="propertyName">The property to sort by, e.g. ProductID or ProductName</param>
        /// <param name="ascending">Determines sort direction</param>
        /// <returns></returns>
        public static IEnumerable<T> SortBy<T>(this IQueryable<T> source, string propertyName, bool ascending)
        {
            // If ascending is equal to true then sort direction will be ascending otherwise it'll be descending
            string sortDirection = @ascending ? "OrderBy" : "OrderByDescending";

            var parameter = Expression.Parameter(source.ElementType, String.Empty);
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            var methodCallExpression = Expression.Call(typeof(Queryable), sortDirection,
                new[] { source.ElementType, property.Type },
                source.Expression, Expression.Quote(lambda));

            var query = source.Provider.CreateQuery<T>(methodCallExpression);

            return query.AsEnumerable();
        }
    }
}
