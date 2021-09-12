using SereneApi.Core.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SereneApi.Handlers.Rest.Queries
{
    /// <inheritdoc cref="IQueryFactory"/>
    internal class QueryFactory : IQueryFactory
    {
        private readonly ITransformationService _transformer;

        public QueryFactory(ITransformationService transformer)
        {
            _transformer = transformer;
        }

        /// <inheritdoc><cref>IQueryFactory.Build</cref></inheritdoc>
        public string Build<TQueryable>(TQueryable query) where TQueryable : class
        {
            Dictionary<string, string> querySections = _transformer.BuildDictionary(query);

            return BuildQueryString(querySections);
        }

        /// <inheritdoc><cref>IQueryFactory.Build</cref></inheritdoc>
        public string Build<TQueryable>(TQueryable query, Expression<Func<TQueryable, object>> selector) where TQueryable : class
        {
            Dictionary<string, string> querySections = _transformer.BuildDictionary(query, selector);

            return BuildQueryString(querySections);
        }

        public string Build(Dictionary<string, string> query)
        {
            return BuildQueryString(query);
        }

        /// <summary>
        /// Builds the query section.
        /// </summary>
        private static string BuildQuerySection(KeyValuePair<string, string> querySection)
        {
            return $"{querySection.Key}={querySection.Value}";
        }

        /// <summary>
        /// Builds the query string using the supplied array of strings.
        /// </summary>
        /// <param name="querySections">Each string index represents an element in the query.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        private static string BuildQueryString(Dictionary<string, string> querySections)
        {
            if (querySections == null)
            {
                throw new ArgumentNullException(nameof(querySections));
            }

            // No sections return empty string.
            if (querySections.Count == 0)
            {
                return null;
            }

            // There is only one Section so we return the first section.
            if (querySections.Count == 1)
            {
                KeyValuePair<string, string> querySection = querySections.First();

                return $"?{BuildQuerySection(querySection)}";
            }

            StringBuilder queryBuilder = new StringBuilder();

            // AddDependency the question mark to the front of the query.
            queryBuilder.Append("?");

            foreach (KeyValuePair<string, string> querySection in querySections)
            {
                queryBuilder.Append($"{BuildQuerySection(querySection)}&");
            }

            string queryString = queryBuilder.ToString();

            // If ampersand is the last character of the query string remove it.
            if (queryString.Last() == '&')
            {
                queryString = queryString.Remove(queryString.Length - 1);
            }

            return queryString;
        }
    }
}