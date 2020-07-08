using System;

namespace SereneApi.Abstractions.Queries.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryConverterAttribute: Attribute
    {
        public IQueryConverter Converter { get; }

        public QueryConverterAttribute(Type converter)
        {
            Converter = (IQueryConverter)Activator.CreateInstance(converter);
        }
    }
}
