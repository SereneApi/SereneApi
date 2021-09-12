using System;

namespace SereneApi.Core.Transformation.Transformers
{
    public class BasicObjectToStringTransformer : ObjectToStringTransformer<object>
    {
        protected override string Transform(object value)
        {
            // If object is of a DateTime Value we will convert it to once.
            if (value is DateTime dateTimeQuery)
            {
                // The DateTime contains a TimeSpan so we'll include that in the query
                if (dateTimeQuery.TimeOfDay != TimeSpan.Zero)
                {
                    return dateTimeQuery.ToString("yyyy-MM-dd HH:mm:ss");
                }

                // The DateTime doesn't contain a TimeSpan so we will forgo it.
                return dateTimeQuery.ToString("yyyy-MM-dd");
            }

            // All other objects will use the default ToString implementation.
            return value.ToString();
        }
    }
}