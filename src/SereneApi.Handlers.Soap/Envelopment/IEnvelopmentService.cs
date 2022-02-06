using SereneApi.Core.Http.Content;
using System.Collections.Generic;

namespace SereneApi.Handlers.Soap.Envelopment
{
    public interface IEnvelopmentService
    {
        IRequestContent Envelop(Dictionary<string, string> parameters, string serviceName, string prefix = null, string namespaceUri = null);
    }
}