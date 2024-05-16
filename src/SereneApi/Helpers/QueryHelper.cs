using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SereneApi.Helpers
{
    internal static class QueryHelper
    {
        public static string BuildQueryString(Dictionary<string, string> querySections)
        {
            if (querySections == null)
            {
                throw new ArgumentNullException(nameof(querySections));
            }

            if (querySections.Count == 0)
            {
                return string.Empty;
            }

            if (querySections.Count == 1)
            {
                KeyValuePair<string, string> querySection = querySections.First();

                return $"?{BuildQuerySection(querySection)}";
            }

            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.Append("?");

            foreach (KeyValuePair<string, string> querySection in querySections)
            {
                queryBuilder.Append($"{BuildQuerySection(querySection)}&");
            }

            string queryString = queryBuilder.ToString();

            if (queryString.Last() == '&')
            {
                queryString = queryString.Remove(queryString.Length - 1);
            }

            return queryString;
        }

        private static string BuildQuerySection(KeyValuePair<string, string> querySection)
            => $"{querySection.Key}={querySection.Value}";
    }
}
