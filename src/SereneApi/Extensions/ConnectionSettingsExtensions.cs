using SereneApi.Abstractions.Connection;
using SereneApi.Requests.Types;

namespace SereneApi.Extensions
{
    public static class ConnectionSettingsExtensions
    {
        public static ApiRequest GenerateApiRequest(this IConnectionSettings connection)
        {
            ApiRequest apiRequest = new ApiRequest
            {
                Resource = connection.Resource,
                ResourcePath = connection.ResourcePath
            };

            return apiRequest;
        }
    }
}
