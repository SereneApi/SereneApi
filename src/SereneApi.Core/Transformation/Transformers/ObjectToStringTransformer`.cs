using System;

namespace SereneApi.Core.Transformation.Transformers
{
    public abstract class ObjectToStringTransformer<T> : IObjectToStringTransformer
    {
        protected abstract string Transform(T value);

        public string TransformValue(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value is T queryValue)
            {
                return Transform(queryValue);
            }

            throw new ArgumentException($"An incorrect value was provided for {GetType().FullName}");
        }
    }
}
