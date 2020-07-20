using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Queries
{
    /// <inheritdoc cref="IQueryConverter"/>
    /// <typeparam name="T">The type this converter accepts.</typeparam>
    public interface IQueryConverter<in T>: IQueryConverter
    {
        /// <summary>
        /// Converts the specified object into a string.
        /// </summary>
        /// <param name="value">The value to be converted into a string.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        string Convert([NotNull] T value);
    }
}
