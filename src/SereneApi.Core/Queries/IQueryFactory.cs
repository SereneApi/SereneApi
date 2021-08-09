using SereneApi.Core.Queries.Attributes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SereneApi.Core.Queries
{
    /// <summary>
    /// Builds a query string from the supplied values.
    /// </summary>
    /// <remarks>
    /// The <see cref="QueryKeyAttribute"/> can be used to set the queries key.<br/>
    /// The <see cref="QueryConverterAttribute"/> can be used to specify how the query value will be converted.
    /// </remarks>
    public interface IQueryFactory
    {
        /// <summary>
        /// Builds the query string using all public properties.
        /// </summary>
        /// <typeparam name="TQueryable">The type to be converted into the query.</typeparam>
        /// <param name="query">The instantiated type that the query values will be generated from.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        string Build<TQueryable>(TQueryable query);

        /// <summary>
        /// Builds the query string using the selected properties form the anonymous type.
        /// </summary>
        /// <typeparam name="TQueryable">The type to be converted into the query.</typeparam>
        /// <param name="query">The instantiated type that the selector will use to create the anonymous type.</param>
        /// <param name="selector">An anonymous type containing the desired properties to be appended to the query.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        string Build<TQueryable>(TQueryable query, Expression<Func<TQueryable, object>> selector);

        string Build(Dictionary<string, string> query);

        Dictionary<string, string> BuildDictionary<TQueryable>(TQueryable query);
        Dictionary<string, string> BuildDictionary<TQueryable>(TQueryable query, Expression<Func<TQueryable, object>> selector);
    }
}
