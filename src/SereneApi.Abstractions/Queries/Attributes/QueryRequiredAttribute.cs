using System;

namespace SereneApi.Abstractions.Queries.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryRequiredAttribute : Attribute
    {
        public bool IsRequired { get; }

        public QueryRequiredAttribute(bool required = true)
        {
            IsRequired = required;
        }
    }
}
