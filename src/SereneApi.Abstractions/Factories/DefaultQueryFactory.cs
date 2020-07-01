using SereneApi.Abstractions.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SereneApi.Abstractions.Factories
{
    /// <inheritdoc cref="IQueryFactory"/>
    public sealed class DefaultQueryFactory: IQueryFactory
    {
        private readonly ObjectToStringFormatter _formatter;

        /// <summary>
        /// Instantiates a new instance of the <see cref="DefaultQueryFactory"/> using the default built in <see cref="ObjectToStringFormatter"/>.
        /// </summary>
        public DefaultQueryFactory()
        {
            _formatter = DefaultQueryFormatter;
        }

        /// <summary>
        /// Instantiates a new instance of the <see cref="DefaultQueryFactory"/> using the supplied <see cref="ObjectToStringFormatter"/>.
        /// </summary>
        public DefaultQueryFactory(ObjectToStringFormatter formatter)
        {
            _formatter = formatter;
        }

        /// <inheritdoc>
        ///     <cref>IQueryFactory.Build</cref>
        /// </inheritdoc>
        public string Build<TQueryable>(TQueryable query)
        {
            List<string> querySections = new List<string>();

            PropertyInfo[] properties = typeof(TQueryable).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach(PropertyInfo property in properties)
            {
                object value = property.GetValue(query);

                // TODO: Add Requirement check.
                if(value == null)
                {
                    continue;
                }

                string valueString = _formatter(value);

                if(!string.IsNullOrEmpty(valueString))
                {
                    querySections.Add(BuildQuerySection(property.Name, valueString));
                }
            }

            return BuildQueryString(querySections);
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

                object value = property.GetValue(query);

                if(value == null)
                {
                    // The property was not set, so it is skipped.
                    continue;
                }

                string valueString = _formatter(value);

                if(!string.IsNullOrEmpty(valueString))
                {
                    querySections.Add(BuildQuerySection(property.Name, valueString));
                }
            }

            return BuildQueryString(querySections);
        }

        /// <summary>
        /// Builds the query string using the supplied array of strings.
        /// </summary>
        /// <param name="querySections">Each string index represents an element in the query.</param>
        private static string BuildQueryString(IReadOnlyList<string> querySections)
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
