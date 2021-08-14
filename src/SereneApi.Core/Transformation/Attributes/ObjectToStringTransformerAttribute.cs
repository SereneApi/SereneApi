using SereneApi.Core.Transformation.Transformers;
using System;
using System.Linq;

namespace SereneApi.Core.Transformation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ObjectToStringTransformerAttribute : Attribute
    {
        public IObjectToStringTransformer Transformer { get; }

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

            Transformer = (IObjectToStringTransformer)Activator.CreateInstance(converter);
        }
    }
}
