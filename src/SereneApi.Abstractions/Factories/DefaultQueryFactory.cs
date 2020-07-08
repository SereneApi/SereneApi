using SereneApi.Abstractions.Delegates;
using SereneApi.Abstractions.Queries.Attributes;
using SereneApi.Abstractions.Queries.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SereneApi.Abstractions.Factories
{
    /// <inheritdoc cref="IQueryFactory"/>
    internal class DefaultQueryFactory: IQueryFactory
    {
        private readonly ObjectToStringFormatter _formatter;

        /// <summary>
        /// Instantiates a new instance of the <see cref="DefaultQueryFactory"/> using the default built in <see cref="ObjectToStringFormatter"/>.
        /// </summary>
        public DefaultQueryFactory()
        {
            _formatter = DefaultQueryFormatter;
        }

        /// <inheritdoc>
        ///     <cref>IQueryFactory.Build</cref>
        /// </inheritdoc>
        public string Build<TQueryable>(TQueryable query)
        {
            PropertyInfo[] queryProperties = typeof(TQueryable).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            List<string> querySections = queryProperties.Select(p => ExtractQueryString(p, query)).Where(q => !string.IsNullOrWhiteSpace(q)).ToList();

            return BuildQuerystring(querySections);
        }

        /// <inheritdoc>
        ///     <cref>IQueryFactory.Build</cref>
        /// </inheritdoc>
        public string Build<TQueryable>(TQueryable query, Expression<Func<TQueryable, object>> selector)
        {
            List<string> querySections = new List<string>();

            if(!(selector.Body is NewExpression body))
            {
                return string.Empty;
            }

            foreach(Expression expression in body.Arguments)
            {
                if(!(expression is MemberExpression member))
                {
                    continue;
                }

                PropertyInfo property = (PropertyInfo)member.Member;

                string queryString = ExtractQueryString(property, query);

                if(string.IsNullOrWhiteSpace(queryString))
                {
                    continue;
                }

                querySections.Add(queryString);
            }

            return BuildQuerystring(querySections);
        }

        private string ExtractQueryString<TQueryable>(PropertyInfo queryProperty, TQueryable query)
        {
            string queryString = string.Empty;

            object queryValue = queryProperty.GetValue(query);

            // Requirement only matters if no value was supplied.
            RequiredAttribute requiredAttribute = queryProperty.GetCustomAttribute<RequiredAttribute>();

            bool required = false;

            if(requiredAttribute != null)
            {
                requiredAttribute?.Validate(queryValue, queryProperty.Name);

                required = true;
            }

            if(queryValue != null)
            {
                QueryConverterAttribute converter = queryProperty.GetCustomAttribute<QueryConverterAttribute>();

                if(converter == null)
                {
                    queryString = _formatter(queryValue);
                }
                else
                {
                    queryString = converter.Converter.Convert(queryValue);
                }

                if(string.IsNullOrWhiteSpace(queryString))
                {
                    if(required)
                    {
                        throw new QueryRequiredException(queryProperty.Name);
                    }

                    queryString = string.Empty;
                }
            }

            if(string.IsNullOrWhiteSpace(queryString))
            {
                return queryString;
            }

            queryProperty.GetCustomAttribute<MinLengthAttribute>()?.Validate(queryString, queryProperty.Name);
            queryProperty.GetCustomAttribute<MaxLengthAttribute>()?.Validate(queryString, queryProperty.Name);

            string queryKey;

            QueryKeyAttribute key = queryProperty.GetCustomAttribute<QueryKeyAttribute>();

            if(key == null)
            {
                queryKey = queryProperty.Name;
            }
            else
            {
                queryKey = key.Value;
            }

            return BuildQuerySection(queryKey, queryString);
        }

        /// <summary>
        /// Builds the query string using the supplied array of strings.
        /// </summary>
        /// <param name="querySections">Each string index represents an element in the query.</param>
        private static string BuildQuerystring(IReadOnlyList<string> querySections)
        {
            // No sections return empty string.
            if(querySections.Count == 0)
            {
                return null;
            }

            // There is only one Section so we return the first section.
            if(querySections.Count == 1)
            {
                return $"?{querySections[0]}";
            }

            StringBuilder queryBuilder = new StringBuilder();

            // AddDependency the question mark to the front of the query.
            queryBuilder.Append("?");

            // Enumerate all indexes except for the last index.
            for(int i = 0; i < querySections.Count - 1; i++)
            {
                // Attach the query section and append an ampersand as we are adding more sections.
                queryBuilder.Append($"{querySections[i]}&");
            }

            // Last section so we don't need an ampersand.
            queryBuilder.Append(querySections.Last());

            return queryBuilder.ToString();
        }

        /// <summary>
        /// Builds the query section using the supplied name and value string.
        /// </summary>
        /// <param name="name">The key to be used in the query section.</param>
        /// <param name="value">The value to be used in the query section.</param>
        private static string BuildQuerySection(string name, string value)
        {
            return $"{name}={value}";
        }

        /// <summary>
        /// The default formatter that is used to convert objects into strings.
        /// </summary>
        private static string DefaultQueryFormatter(object queryObject)
        {
            // If object is of a DateTime Value we will convert it to once.
            if(queryObject is DateTime dateTimeQuery)
            {
                // The DateTime contains a TimeSpan so we'll include that in the query
                if(dateTimeQuery.TimeOfDay != TimeSpan.Zero)
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
