using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Abstractions.Queries.Attributes
{
    /// <summary>
    /// Provides a converter for generation of a query property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryConverterAttribute: Attribute
    {
        /// <summary>
        /// The specified converter.
        /// </summary>
        public IQueryConverter Converter { get; }

        /// <summary>
        /// Creates a new instance of <see cref="QueryConverterAttribute"/>.
        /// </summary>
        /// <param name="converter">The type which will perform the conversion.</param>
        /// <exception cref="ArgumentException">Thrown if the specified type is no a class or does not implement <see cref="IQueryConverter"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        public QueryConverterAttribute([NotNull] Type converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if(!converter.IsClass)
            {
                throw new ArgumentException($"{converter.FullName} must be a class");
            }

            if (!converter.GetInterfaces().Contains(typeof(IQueryConverter)))
            {
                throw new ArgumentException($"{converter.FullName} must implement {nameof(IQueryConverter)}");
            }

            Converter = (IQueryConverter)Activator.CreateInstance(converter);
        }
    }
}
