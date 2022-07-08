using System;

namespace SereneApi.Extensions.Mocking.Rest.Helpers
{
    internal static class ParameterHelper
    {
        private const char ParameterStartKey = '{';
        private const char ParameterEndKey = '}';
        private const char ParameterOptionalKey = '?';

        public static bool IsParameter(string value)
        {
            return value.StartsWith(ParameterStartKey) && value.EndsWith(ParameterEndKey);
        }

        public static bool IsParameterOptional(string value)
        {
            if (value.EndsWith(ParameterEndKey))
            {
                return value[^2] == ParameterOptionalKey;
            }

            return value.EndsWith(ParameterOptionalKey);
        }

        public static string GetParameterKey(string value)
        {
            if (!IsParameter(value))
            {
                throw new ArgumentException($"The specified value \"{value}\" is not a parameter");
            }

            // Remove first characters from start and end of string.
            // EG "{MyParameter}" will be turned into "MyParameter"
            return value[1..^1];
        }

        public static string RemoveOptionalKey(string value)
        {
            if (value.EndsWith(ParameterOptionalKey))
            {
                return value.Substring(0, value.Length - 1);
            }

            return value;
        }
    }
}
