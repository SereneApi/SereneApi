using SereneApi.Delegates;
using SereneApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SereneApi
{
    public class QueryFactory : IQueryFactory
    {
        protected readonly ObjectStringFormatter Formatter;

        public QueryFactory()
        {
            Formatter = DefaultQueryFormatter;
        }

        public QueryFactory(ObjectStringFormatter formatter)
        {
            Formatter = formatter;
        }

        /// <summary>
        /// All public properties will be used to build the query.
        /// </summary>
        public virtual string Build<TQueryable>(TQueryable query)
        {
            List<string> querySections = new List<string>();

            PropertyInfo[] properties = typeof(TQueryable).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(query);

                string valueString = Formatter(value);

                if (!string.IsNullOrEmpty(valueString))
                {
                    querySections.Add(BuildQuerySection(property.Name, valueString));
                }
            }

            return BuildQuery(querySections);
        }

        /// <summary>
        /// Select what properties will be used in the query.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual string Build<TQueryable>(TQueryable query, Expression<Func<TQueryable, object>> selector)
        {
            List<string> querySections = new List<string>();

            if (!(selector.Body is NewExpression body))
            {
                return string.Empty;
            }

            foreach (Expression expression in body.Arguments)
            {
                if (!(expression is MemberExpression member))
                {
                    continue;
                }

                PropertyInfo property = (PropertyInfo)member.Member;

                object value = property.GetValue(query);

                string valueString = Formatter(value);

                if (!string.IsNullOrEmpty(valueString))
                {
                    querySections.Add(BuildQuerySection(property.Name, valueString));
                }
            }

            return BuildQuery(querySections);
        }

        protected virtual string BuildQuery(IReadOnlyList<string> querySections)
        {
            // No sections return empty string.
            if (querySections.Count == 0)
            {
                return string.Empty;
            }

            // Their is only one Section so we return the first section.
            if (querySections.Count == 1)
            {
                return querySections[0];
            }

            StringBuilder queryBuilder = new StringBuilder();

            // AddDependency the question mark to the front of the query.
            queryBuilder.Append("?");

            // Enumerate all indexes except for the last index.
            for (int i = 0; i < querySections.Count - 1; i++)
            {
                // Attach the query section and append an ampersand as we are adding more sections.
                queryBuilder.Append($"{querySections[i]}&");
            }

            // Last section so we don't need an ampersand.
            queryBuilder.Append(querySections.Last());

            return queryBuilder.ToString();
        }

        protected virtual string BuildQuerySection(string name, string value)
        {
            return $"{name}={value}";
        }

        /// <summary>
        /// The default formatter that is used for queries.
        /// </summary>
        private static string DefaultQueryFormatter(object queryObject)
        {
            // If object is of a DateTime Value we will convert it to once.
            if (queryObject is DateTime dateTimeQuery)
            {
                // The DateTime contains a TimeSpan so we'll include that in the query
                if (dateTimeQuery.TimeOfDay != TimeSpan.MinValue)
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
