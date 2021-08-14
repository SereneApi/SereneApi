using SereneApi.Core.Transformation.Attributes;
using SereneApi.Core.Transformation.Exceptions;
using SereneApi.Core.Transformation.Transformers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace SereneApi.Core.Transformation
{
    public class TransformationService : ITransformationService, IDisposable
    {
        private readonly IObjectToStringTransformer _defaultTransformer;

        public TransformationService(IObjectToStringTransformer transformer = null)
        {
            _defaultTransformer = transformer ?? new BasicObjectToStringTransformer();
        }

        public Dictionary<string, string> BuildDictionary<T>(T value) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Dictionary<string, string> propertySections = new Dictionary<string, string>();

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                KeyValuePair<string, string> querySection = GetPropertyKeyValuePair(value, property);

                if (string.IsNullOrWhiteSpace(querySection.Value))
                {
                    continue;
                }

                propertySections.Add(querySection.Key, querySection.Value);
            }

            return propertySections;
        }

        public Dictionary<string, string> BuildDictionary<T>(T value, Expression<Func<T, object>> propertySelector) where T : class
        {
            if (propertySelector.Body is not NewExpression body)
            {
                throw new ArgumentException($"{nameof(propertySelector)} must be a {nameof(MemberExpression)}");
            }

            Dictionary<string, string> propertySections = new Dictionary<string, string>();

            foreach (Expression expression in body.Arguments)
            {
                if (expression is not MemberExpression member)
                {
                    continue;
                }

                PropertyInfo property = (PropertyInfo)member.Member;

                KeyValuePair<string, string> querySection = GetPropertyKeyValuePair(value, property);

                if (string.IsNullOrWhiteSpace(querySection.Value))
                {
                    continue;
                }

                propertySections.Add(querySection.Key, querySection.Value);
            }

            return propertySections;
        }

        private KeyValuePair<string, string> GetPropertyKeyValuePair<T>(T value, PropertyInfo property) where T : class
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string transformedValue = string.Empty;

            object objectValue = property.GetValue(value);

            // Requirement only matters if no value was supplied.
            RequiredAttribute requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();

            bool required = false;

            if (requiredAttribute != null)
            {
                requiredAttribute.Validate(objectValue, property.Name);

                required = true;
            }

            if (objectValue != null)
            {
                ObjectToStringTransformerAttribute attribute = property.GetCustomAttribute<ObjectToStringTransformerAttribute>();

                if (attribute == null)
                {
                    transformedValue = _defaultTransformer.TransformValue(objectValue);
                }
                else
                {
                    transformedValue = attribute.Transformer.TransformValue(objectValue);
                }

                if (string.IsNullOrWhiteSpace(transformedValue))
                {
                    if (required)
                    {
                        throw new PropertyRequiredException(property.Name);
                    }

                    transformedValue = string.Empty;
                }
            }

            if (string.IsNullOrWhiteSpace(transformedValue))
            {
                return new KeyValuePair<string, string>();
            }

            property.GetCustomAttribute<MinLengthAttribute>()?.Validate(transformedValue, property.Name);
            property.GetCustomAttribute<MaxLengthAttribute>()?.Validate(transformedValue, property.Name);

            NameAttribute name = property.GetCustomAttribute<NameAttribute>();

            string queryKey = name == null ? property.Name : name.Value;

            return new KeyValuePair<string, string>(queryKey, transformedValue);
        }

        public void Dispose()
        {
        }
    }
}
