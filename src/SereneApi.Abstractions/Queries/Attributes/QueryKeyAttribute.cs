using System;

namespace SereneApi.Abstractions.Queries.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryKeyAttribute: Attribute
    {
        public string Value { get; }

        public QueryKeyAttribute(string value)
        {
            Value = value;
        }
    }
}
