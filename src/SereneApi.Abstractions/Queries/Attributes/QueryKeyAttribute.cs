using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Queries.Attributes
{
    /// <summary>
    /// Specifies the key to be used for the queries value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryKeyAttribute: Attribute
    {
        /// <summary>
        /// The key to be used for the queries value.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Creates a new instance of <see cref="QueryKeyAttribute"/>.
        /// </summary>
        /// <param name="key">The key to be used for the query value.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public QueryKeyAttribute([NotNull] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            Key = key;
        }
    }
}
