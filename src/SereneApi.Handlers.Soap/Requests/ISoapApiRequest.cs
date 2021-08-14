using System.Collections.Generic;
using SereneApi.Core.Requests;

namespace SereneApi.Handlers.Soap.Requests
{
    public interface ISoapApiRequest : IApiRequest
    {
        string Service { get; }

        Dictionary<string, string> Parameters { get; }
    }
}
