using System;

namespace SereneApi.Abstractions.Queries.Exceptions
{
    /// <summary>
    /// Thrown when a query requires a specific property to be provided.
    /// </summary>
    public class QueryRequiredException: Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="QueryRequiredException"/>.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        public QueryRequiredException(string propertyName) : base($"{propertyName} must be provided as it is a required part of the query", new ArgumentNullException(propertyName))
        {
        }
    }
}
