using System;

namespace SereneApi.Abstractions.Helpers
{
    /// <summary>
    /// Contains methods to help with sources
    /// </summary>
    public static class SourceHelpers
    {
        /// <summary>
        /// Ensures that a / is appended to the end of the value.
        /// </summary>
        public static string EnsureSourceSlashTermination(string value)
        {
            const char termination = '/';

            if(String.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            int lastCharIndex = value.Length - 1;

            if(value[lastCharIndex] != termination)
            {
                value += termination;
            }

            return value;
        }

        /// <summary>
        /// Ensures that a / is appended to the end of the value.
        /// </summary>
        public static Uri EnsureSourceSlashTermination(Uri value)
        {
            string valueString = EnsureSourceSlashTermination(value.ToString());

            return new Uri(valueString);
        }

        /// <summary>
        /// Ensures that there is no / appended to the end of the value.
        /// </summary>
        public static string EnsureSourceNoSlashTermination(string value)
        {
            if(String.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            int index = value.Length - 1;

            while(index >= 1 && value[index] == '/')
            {
                value = value.Substring(0, index);

                index--;
            }

            return value;
        }

        /// <summary>
        /// Ensures that there is no / appended to the end of the value.
        /// </summary>
        public static Uri EnsureSourceNoSlashTermination(Uri value)
        {
            string valueString = EnsureSourceNoSlashTermination(value.ToString());

            return new Uri(valueString);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the source does not end in a /
        /// </summary>
        public static void CheckIfValid(string source)
        {
            int lastCharIndex = source.Length - 1;

            if(source[lastCharIndex] != '/')
            {
                throw new ArgumentException("The HttpClient BaseAddress must end with a /");
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the source does not end in a /
        /// </summary>
        public static void CheckIfValid(Uri source)
        {
            CheckIfValid(source.ToString());
        }

        /// <summary>
        /// If the resource path is null or whitespace the default value will be used.
        /// If the string contains anything other than whitespace the value provided will be used.
        /// Setting an Empty string will disable the default value.
        /// </summary>
        public static string UseOrGetDefaultResourcePath(string resourcePath)
        {
            // If an empty string is supplied, the default value is disabled.
            if(resourcePath == String.Empty)
            {
                return resourcePath;
            }

            // Null or whitespace strings will enabled the default.
            if(String.IsNullOrWhiteSpace(resourcePath))
            {
                resourcePath = Defaults.Handler.ResourcePath;
            }

            resourcePath = SourceHelpers.EnsureSourceSlashTermination(resourcePath);

            return resourcePath;
        }
    }
}
