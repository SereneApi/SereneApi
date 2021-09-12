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
    public class TransformationService : ITransformationService
    {
        private readonly IObjectToStringTransformer _defaultObjectToStringTransformer;

        private readonly IStringToObjectTransformer _defaultStringToObjectTransformer;

        public TransformationService(IObjectToStringTransformer objectTransformer, IStringToObjectTransformer stringTransformer)
        {
            _defaultObjectToStringTransformer = objectTransformer;
            _defaultStringToObjectTransformer = stringTransformer;
        }

        public Dictionary<string, string> BuildDictionary<T>(T value) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            SortedDictionary<string, string> propertySections = new SortedDictionary<string, string>();

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

            return new Dictionary<string, string>(propertySections);
        }

        public Dictionary<string, string> BuildDictionary<T>(T value, Expression<Func<T, object>> propertySelector) where T : class
        {
            if (propertySelector.Body is not NewExpression body)
            {
                throw new ArgumentException($"{nameof(propertySelector)} must be a {nameof(MemberExpression)}");
            }

            SortedDictionary<string, string> propertySections = new SortedDictionary<string, string>();

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

            return new Dictionary<string, string>(propertySections);
        }

        public T BuildObject<T>(Dictionary<string, string> values)
        {
            T instance = Activator.CreateInstance<T>();

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                NameAttribute name = property.GetCustomAttribute<NameAttribute>();

                string propertyName = property.Name;

                if (name != null)
                {
                    propertyName = name.Value;
                }

                object value = null;

                if (values.TryGetValue(propertyName, out string stringValue))
                {
                    StringToObjectTransformerAttribute transformer = property.GetCustomAttribute<StringToObjectTransformerAttribute>();

                    if (transformer == null)
                    {
                        value = _defaultStringToObjectTransformer.TransformValue(stringValue, property.PropertyType);
                    }
                    else
                    {
                        value = transformer.Transform(stringValue, property.PropertyType);
                    }
                }

                RequiredAttribute requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();

                requiredAttribute?.Validate(stringValue, propertyName);

                property.SetValue(instance, value);
            }

            return instance;
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
                    transformedValue = _defaultObjectToStringTransformer.TransformValue(objectValue);
                }
                else
                {
                    transformedValue = attribute.Transform(objectValue);
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
    }
}