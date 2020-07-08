using System;

namespace SereneApi.Abstractions.Queries.Exceptions
{
    public class QueryRequiredException: Exception
    {
        public QueryRequiredException(string propertyName) : base($"{propertyName} must be provided as it is a required part of the query", new ArgumentNullException(propertyName))
        {
        }
    }
}
