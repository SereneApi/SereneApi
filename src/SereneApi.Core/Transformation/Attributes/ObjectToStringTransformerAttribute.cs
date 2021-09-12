using SereneApi.Core.Transformation.Transformers;
using System;
using System.Linq;

namespace SereneApi.Core.Transformation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ObjectToStringTransformerAttribute : Attribute
    {
        private readonly IObjectToStringTransformer _transformer;

        public ObjectToStringTransformerAttribute(Type converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (!converter.IsClass)
            {
                throw new ArgumentException($"{converter.FullName} must be a class");
            }

            if (!converter.GetInterfaces().Contains(typeof(IObjectToStringTransformer)))
            {
                throw new ArgumentException($"{converter.FullName} must implement {nameof(IObjectToStringTransformer)}");
            }

            _transformer = (IObjectToStringTransformer)Activator.CreateInstance(converter);
        }

        public string Transform(object value)
        {
            return _transformer.TransformValue(value);
        }
    }
}