using System;

namespace SereneApi.Abstractions.Queries.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryConverterAttribute: Attribute
    {
        public IQueryConverter Converter { get; }

        public QueryConverterAttribute(Type converter)
        {
            if(!converter.IsClass)
            {
                throw new ArgumentException($"{converter.FullName} must be a class");
            }

            Converter = (IQueryConverter)Activator.CreateInstance(converter);
        }
    }
}
