using System;

namespace SereneApi.Abstractions.Queries
{
    public abstract class QueryConverter1<T>: IQueryConverter
    {
        protected abstract string Convert(T value);

        public string Convert(object value)
        {
            if (value is T queryValue)
            {
                return Convert(queryValue);
            }

            throw new ArgumentException($"An incorrect value was provided for {GetType().FullName}");
        }
    }
}
