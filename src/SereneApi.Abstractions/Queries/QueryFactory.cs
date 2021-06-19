using SereneApi.Abstractions.Queries.Attributes;
using SereneApi.Abstractions.Queries.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SereneApi.Abstractions.Queries
{
    /// <inheritdoc cref="IQueryFactory"/>
    public class QueryFactory : IQueryFactory
    {
        private readonly ObjectToStringFormatter _formatter;

        /// <summary>
        /// Instantiates a new instance of the <see cref="QueryFactory"/> using the default built in <see cref="ObjectToStringFormatter"/>.
        /// </summary>
        /// <param name="formatter">The default formatter, overridden by <see cref="QueryConverterAttribute"/>.</param>
        public QueryFactory([AllowNull] ObjectToStringFormatter formatter = null)
        {
            _formatter = formatter ?? DefaultQueryFormatter;
        }

        /// <inheritdoc>
        ///     <cref>IQueryFactory.Build</cref>
        /// </inheritdoc>
        public string Build<TQueryable>(TQueryable query)
        {
            Dictionary<string, string> querySections = BuildDictionary(query);

            return BuildQueryString(querySections);
        }

        /// <inheritdoc>
        ///     <cref>IQueryFactory.Build</cref>
        /// </inheritdoc>
        public string Build<TQueryable>(TQueryable query, Expression<Func<TQueryable, object>> selector)
        {
            Dictionary<string, string> querySections = BuildDictionary(query, selector);

            return BuildQueryString(querySections);
        }

        public string Build(Dictionary<string, string> query)
        {
            return BuildQueryString(query);
        }

        public Dictionary<string, string> BuildDictionary<TQueryable>(TQueryable query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            Dictionary<string, string> querySections = new Dictionary<string, string>();

            PropertyInfo[] queryProperties = typeof(TQueryable).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo queryProperty in queryProperties)
            {
                KeyValuePair<string, string> querySection = ExtractQuerySection(queryProperty, query);

                if (string.IsNullOrWhiteSpace(querySection.Value))
                {
                    continue;
                }

                querySections.Add(querySection.Key, querySection.Value);
            }

            return querySections;
        }

        public Dictionary<string, string> BuildDictionary<TQueryable>(TQueryable query, Expression<Func<TQueryable, object>> selector)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (!(selector.Body is NewExpression body))
            {
                return null;
            }

            Dictionary<string, string> querySections = new Dictionary<string, string>();

            foreach (Expression expression in body.Arguments)
            {
                if (!(expression is MemberExpression member))
                {
                    continue;
                }

                PropertyInfo property = (PropertyInfo)member.Member;

                KeyValuePair<string, string> querySection = ExtractQuerySection(property, query);

                if (string.IsNullOrWhiteSpace(querySection.Value))
                {
                    continue;
                }

                querySections.Add(querySection.Key, querySection.Value);
            }

            return querySections;
        }

        /// <summary>
        /// Gets the query value from the provided property.
        /// </summary>
        /// <typeparam name="TQueryable">The type the value will be extracted from.</typeparam>
        /// <param name="queryProperty">The property containing the specific value.</param>
        /// <param name="query">The instantiated query that the value will be extracted from.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        private KeyValuePair<string, string> ExtractQuerySection<TQueryable>(PropertyInfo queryProperty, TQueryable query)
        {
            if (queryProperty == null)
            {
                throw new ArgumentNullException(nameof(queryProperty));
            }

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            string queryString = string.Empty;

            object queryValue = queryProperty.GetValue(query);

            // Requirement only matters if no value was supplied.
            RequiredAttribute requiredAttribute = queryProperty.GetCustomAttribute<RequiredAttribute>();

            bool required = false;

            if (requiredAttribute != null)
            {
                requiredAttribute.Validate(queryValue, queryProperty.Name);

                required = true;
            }

            if (queryValue != null)
            {
                QueryConverterAttribute converter = queryProperty.GetCustomAttribute<QueryConverterAttribute>();

                if (converter == null)
                {
                    queryString = _formatter(queryValue);
                }
                else
                {
                    queryString = converter.Converter.Convert(queryValue);
                }

                if (string.IsNullOrWhiteSpace(queryString))
                {
                    if (required)
                    {
                        throw new RequiredQueryElementException(queryProperty.Name);
                    }

                    queryString = string.Empty;
                }
            }

            if (string.IsNullOrWhiteSpace(queryString))
            {
                return new KeyValuePair<string, string>();
            }

            queryProperty.GetCustomAttribute<MinLengthAttribute>()?.Validate(queryString, queryProperty.Name);
            queryProperty.GetCustomAttribute<MaxLengthAttribute>()?.Validate(queryString, queryProperty.Name);

            string queryKey;

            QueryKeyAttribute key = queryProperty.GetCustomAttribute<QueryKeyAttribute>();

            if (key == null)
            {
                queryKey = queryProperty.Name;
            }
            else
            {
                queryKey = key.Key;
            }

            return new KeyValuePair<string, string>(queryKey, queryString);
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

        /// <summary>
        /// Builds the query section.
        /// </summary>
        private static string BuildQuerySection(KeyValuePair<string, string> querySection)
        {
            return $"{querySection.Key}={querySection.Value}";
        }

        /// <summary>
        /// The default formatter that is used to convert objects into strings.
        /// </summary>
        private static string DefaultQueryFormatter(object queryObject)
        {
            // If object is of a DateTime Value we will convert it to once.
            if (queryObject is DateTime dateTimeQuery)
            {
                // The DateTime contains a TimeSpan so we'll include that in the query
                if (dateTimeQuery.TimeOfDay != TimeSpan.Zero)
                {
                    return dateTimeQuery.ToString("yyyy-MM-dd HH:mm:ss");
                }

                // The DateTime doesn't contain a TimeSpan so we will forgo it.
                return dateTimeQuery.ToString("yyyy-MM-dd");
            }

            // All other objects will use the default ToString implementation.
            return queryObject.ToString();
        }
    }
}
