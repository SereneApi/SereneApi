using SereneApi.Core.Requests;
using System.Collections.Generic;

namespace SereneApi.Handlers.Soap.Requests
{
    public interface ISoapApiRequest : IApiRequest
    {
        string Service { get; }

        Dictionary<string, string> Parameters { get; }
    }
}
