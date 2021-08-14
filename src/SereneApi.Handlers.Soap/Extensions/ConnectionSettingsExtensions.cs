using SereneApi.Core.Connection;
using SereneApi.Handlers.Soap.Requests.Types;

namespace SereneApi.Handlers.Soap.Extensions
{
    public static class ConnectionSettingsExtensions
    {
        public static SoapApiRequest GenerateApiRequest(this IConnectionSettings connection)
        {
            SoapApiRequest apiRequest = new SoapApiRequest
            {
                Resource = connection.Resource,
                ResourcePath = connection.ResourcePath
            };

            return apiRequest;
        }
    }
}
