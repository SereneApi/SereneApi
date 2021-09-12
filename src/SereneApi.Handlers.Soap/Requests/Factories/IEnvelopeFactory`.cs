using System;

namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IEnvelopeFactory<out TEnvelope>
    {
        IResponseType AndParameters(Action<TEnvelope> envelope);
    }
}