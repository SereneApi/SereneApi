using System;
using System.Linq;

namespace SereneApi.Core.Transformation.Transformers
{
    public class BasicStringToObjectTransformer : IStringToObjectTransformer
    {
        public object TransformValue(string value, Type toType)
        {
            if (toType == typeof(string))
            {
                return value;
            }

            if (toType == typeof(char))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return new char();
                }

                return value.First();
            }

            if (toType == typeof(int))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "0";
                }

                return int.Parse(value);
            }

            if (toType == typeof(long))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "0";
                }

                return long.Parse(value);
            }

            if (toType == typeof(float))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "0";
                }

                return float.Parse(value);
            }

            if (toType == typeof(short))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "0";
                }

                return short.Parse(value);
            }

            if (toType == typeof(DateTime))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return DateTime.MinValue;
                }

                return DateTime.Parse(value);
            }

            if (toType == typeof(TimeSpan))
            {
                string[] time = value.Split(':');

                return TimeSpan.Parse($"{time[0]}:{time[1]}:{time[2]}.{time[3]}");
            }

            return value;
        }
    }
}