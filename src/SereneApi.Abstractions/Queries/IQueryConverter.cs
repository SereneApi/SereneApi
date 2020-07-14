using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Queries
{
    /// <summary>
    /// Converts an object into a string to be used when building queries.
    /// </summary>
    public interface IQueryConverter
    {
        /// <summary>
        /// Converts the specified object into a string.
        /// </summary>
        /// <param name="value">The value to be converted into a string.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        string Convert([NotNull] object value);
    }
}
