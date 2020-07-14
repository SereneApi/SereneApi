using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Queries
{
    public abstract class QueryConverter<T>: IQueryConverter<T>
    {
        /// <inheritdoc>
        ///     <cref>IQueryConverter{T}.Convert</cref>
        /// </inheritdoc>
        public abstract string Convert([NotNull] T value);

        /// <inheritdoc cref="IQueryConverter.Convert"/>
        /// <exception cref="ArgumentException">Thrown when an invalid type is provided.</exception>
        public string Convert([NotNull] object value)
        {
            if(value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if(value is T queryValue)
            {
                return Convert(queryValue);
            }

            throw new ArgumentException($"An incorrect value was provided for {GetType().FullName}");
        }
    }
}
