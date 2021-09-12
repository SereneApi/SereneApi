using SereneApi.Core.Transformation.Transformers;
using System;
using System.Linq;

namespace SereneApi.Core.Transformation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringToObjectTransformerAttribute : Attribute
    {
        private readonly IStringToObjectTransformer _transformer;

        public StringToObjectTransformerAttribute(Type converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (!converter.IsClass)
            {
                throw new ArgumentException($"{converter.FullName} must be a class");
            }

            if (!converter.GetInterfaces().Contains(typeof(IStringToObjectTransformer)))
            {
                throw new ArgumentException($"{converter.FullName} must implement {nameof(IStringToObjectTransformer)}");
            }

            _transformer = (IStringToObjectTransformer)Activator.CreateInstance(converter);
        }

        public object Transform(string value, Type toType)
        {
            return _transformer.TransformValue(value, toType);
        }
    }
}