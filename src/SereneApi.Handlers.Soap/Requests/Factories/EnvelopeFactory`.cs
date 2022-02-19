using SereneApi.Handlers.Soap.Models;
using SereneApi.Handlers.Soap.Requests.Types;
using SereneApi.Handlers.Soap.Serialization;
using System;

namespace SereneApi.Handlers.Soap.Requests.Factories
{
    internal class EnvelopeFactory<TEnvelope> : IEnvelopeFactory<TEnvelope>
    {
        private readonly SoapApiRequest _apiRequest;
        private readonly SoapRequestFactory _requestFactory;
        private readonly ISoapSerializer _serializer;

        public EnvelopeFactory(SoapRequestFactory requestFactory, SoapApiRequest apiRequest, ISoapSerializer serializer)
        {
            _requestFactory = requestFactory;
            _apiRequest = apiRequest;
            _serializer = serializer;
        }

        public IResponseType AndParameters(Action<TEnvelope> envelopeFactory)
        {
            TEnvelope envelope = Activator.CreateInstance<TEnvelope>();

            envelopeFactory.Invoke(envelope);

            SoapEnvelope soapEnvelope = new SoapEnvelope
            {
                Body = envelope
            };

            _apiRequest.Content = _serializer.Serialize(soapEnvelope);

            return _requestFactory;
        }
    }
}