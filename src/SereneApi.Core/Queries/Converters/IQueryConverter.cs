using System;

namespace SereneApi.Core.Queries.Converters
{
    /// <summary>
    /// Converts an object into a query string.
    /// </summary>
    public interface IQueryConverter
    {
        /// <summary>
        /// Converts the specified object into a string.
        /// </summary>
        /// <param name="value">The value to be converted into a string.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        string Convert(object value);
    }
}
