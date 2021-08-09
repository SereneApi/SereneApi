﻿using System;

namespace SereneApi.Core.Helpers
{
    /// <summary>
    /// Contains methods to help with sources
    /// </summary>
    public static class SourceHelpers
    {
        /// <summary>
        /// Ensures that a / is appended to the end of the value.
        /// </summary>
        public static string EnsureSlashTermination(string value)
        {
            const char termination = '/';

            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            int lastCharIndex = value.Length - 1;

            if (value[lastCharIndex] != termination)
            {
                value += termination;
            }

            return value;
        }

        /// <summary>
        /// Ensures that a / is appended to the end of the value.
        /// </summary>
        public static Uri EnsureSlashTermination(Uri value)
        {
            string valueString = EnsureSlashTermination(value.ToString());

            return new Uri(valueString);
        }

        /// <summary>
        /// Ensures that there is no / appended to the end of the value.
        /// </summary>
        public static string EnsureNoSlashTermination(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            int index = value.Length - 1;

            while (index >= 1 && value[index] == '/')
            {
                value = value.Substring(0, index);

                index--;
            }

            return value;
        }

        /// <summary>
        /// Ensures that there is no / appended to the end of the value.
        /// </summary>
        public static Uri EnsureNoSlashTermination(Uri value)
        {
            string valueString = EnsureNoSlashTermination(value.ToString());

            return new Uri(valueString);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the source does not end in a /
        /// </summary>
        public static void CheckIfValid(string source)
        {
            int lastCharIndex = source.Length - 1;

            if (source[lastCharIndex] != '/')
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
    }
}
