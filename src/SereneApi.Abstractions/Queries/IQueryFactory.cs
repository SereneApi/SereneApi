using System;
using System.Linq.Expressions;

namespace SereneApi.Abstractions.Queries
{
    /// <summary>
    /// Builds a query string from the supplied object.
    /// </summary>
    public interface IQueryFactory
    {
        /// <summary>
        /// Builds the query string using all public properties.
        /// </summary>
        /// <typeparam name="TQueryable">The type to be converted into the query.</typeparam>
        /// <param name="query">The instantiated type that the query values will be retrieved from.</param>
        string Build<TQueryable>(TQueryable query);

        /// <summary>
        /// Builds the query string using the selected values selected in the anonymous type.
        /// </summary>
        /// <typeparam name="TQueryable">The type to be converted into the query.</typeparam>
        /// <param name="query">The instantiated type that the selector will use to create the anonymous type.</param>
        /// <param name="selector">An anonymous type will be provided selecting the desired properties from the supplied query type.</param>
        string Build<TQueryable>(TQueryable query, Expression<Func<TQueryable, object>> selector);
    }
}
