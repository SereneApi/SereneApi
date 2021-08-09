using SereneApi.Core.Connection;
using SereneApi.Handlers.Rest.Requests.Types;

namespace SereneApi.Handlers.Rest.Extensions
{
    public static class ConnectionSettingsExtensions
    {
        public static RestApiRequest GenerateApiRequest(this IConnectionSettings connection)
        {
            RestApiRequest apiRequest = new RestApiRequest
            {
                Resource = connection.Resource,
                ResourcePath = connection.ResourcePath
            };

            return apiRequest;
        }
    }
}
