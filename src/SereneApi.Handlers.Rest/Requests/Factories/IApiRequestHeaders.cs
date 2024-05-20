using System;
using System.Collections.Generic;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestHeaders : IApiRequestBody
    {
        IApiRequestBody WithHeaders(Dictionary<string, object> headers);

        IApiRequestBody WithHeaders<THeader>(THeader header);
    }
}
