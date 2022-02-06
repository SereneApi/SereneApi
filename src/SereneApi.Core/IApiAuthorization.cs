using System.Collections.Generic;

namespace SereneApi.Core
{
    public interface IApiAuthorization
    {
        string ClientId { get; }

        List<string> Scopes { get; }
    }
}