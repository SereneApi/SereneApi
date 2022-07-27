using DeltaWare.SDK.Serialization.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SereneApi.Handlers.Rest.Queries
{
    /// <inheritdoc cref="IQuerySerializer"/>
    internal class QuerySerializer : IQuerySerializer
    {
        private readonly IObjectSerializer _objectSerializer;

        public QuerySerializer(IObjectSerializer objectSerializer)
        {
            _objectSerializer = objectSerializer;
        }

        public string Serialize(Dictionary<string, string> query)
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