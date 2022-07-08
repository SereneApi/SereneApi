using SereneApi.Core.Http;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Rest.Extensions
{
    internal static class ConnectionSettingsExtensions
    {
        public static string GetBaseRoute(this IConnectionSettings connection)
        {
            string route = connection.BaseAddress.ToString();

            if (!string.IsNullOrWhiteSpace(connection.ResourcePath))
            {
                if (route.Last() != '/' && connection.ResourcePath.First() != '/')
                {
                    route += '/';
                }

                route += connection.ResourcePath;
            }

            if (string.IsNullOrWhiteSpace(connection.Resource))
            {
                return route;
            }

            if (route.Last() != '/' && connection.Resource.First() != '/')
            {
                route += '/';
            }

            route += connection.Resource;

            return route;
        }
    }
}
