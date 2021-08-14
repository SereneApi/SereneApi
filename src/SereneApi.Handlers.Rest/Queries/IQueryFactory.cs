using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SereneApi.Handlers.Rest.Queries
{
    public interface IQueryFactory
    {
        /// <summary>
        /// Builds the query string using all public properties.
        /// </summary>
        /// <typeparam name="TQueryable">The type to be converted into the query.</typeparam>
        /// <param name="query">The instantiated type that the query values will be generated from.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        string Build<TQueryable>(TQueryable query) where TQueryable : class;

        /// <summary>
        /// Builds the query string using the selected properties form the anonymous type.
        /// </summary>
        /// <typeparam name="TQueryable">The type to be converted into the query.</typeparam>
        /// <param name="query">The instantiated type that the selector will use to create the anonymous type.</param>
        /// <param name="selector">An anonymous type containing the desired properties to be appended to the query.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        string Build<TQueryable>(TQueryable query, Expression<Func<TQueryable, object>> selector) where TQueryable : class;

        string Build(Dictionary<string, string> query);
    }
}
