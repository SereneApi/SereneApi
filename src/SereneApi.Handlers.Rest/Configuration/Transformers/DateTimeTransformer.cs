using DeltaWare.SDK.Core.Transformation;
using System;
using System.Globalization;

namespace SereneApi.Handlers.Rest.Configuration.Transformers
{
    internal class DateTimeTransformer : NullableTransformer<DateTime>
    {
        protected override DateTime TransformToObject(string value, CultureInfo culture = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DateTime.MinValue;
            }

            return DateTime.Parse(value, culture);
        }

        protected override string TransformToString(DateTime value, CultureInfo culture = null)
        {
            if (value.TimeOfDay != TimeSpan.Zero)
            {
                return value.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // The DateTime doesn't contain a TimeSpan so we will forgo it. 
            return value.ToString("yyyy-MM-dd");
        }
    }
}