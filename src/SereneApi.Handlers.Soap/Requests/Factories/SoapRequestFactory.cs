using DeltaWare.Dependencies.Abstractions;
using DeltaWare.SDK.Core.Serialization;
using SereneApi.Core.Requests;
using SereneApi.Core.Versioning;
using SereneApi.Handlers.Soap.Envelopment;
using SereneApi.Handlers.Soap.Models;
using SereneApi.Handlers.Soap.Requests.Types;
using SereneApi.Handlers.Soap.Routing;
using SereneApi.Handlers.Soap.Serialization;
using System;
using System.Collections.Generic;

namespace SereneApi.Handlers.Soap.Requests.Factories
{
    internal class SoapRequestFactory : IRequestFactory, IRequestParameters
    {
        private readonly SoapApiHandler _apiHandler;
        private readonly SoapApiRequest _apiRequest;
        private readonly IDependencyProvider _dependencies;
        private string _service;

        public SoapRequestFactory(SoapApiHandler apiHandler)
        {
            _apiHandler = apiHandler;
            _apiRequest = SoapApiRequest.Create(apiHandler.Connection);
            _apiRequest.Method = Method.Post;

            _dependencies = apiHandler.Settings.Dependencies;
        }

        public IRequestParameters AgainstService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            _service = serviceName;

            return this;
        }

        public IRequestEnvelope AgainstVersion(string version)
        {
            _apiRequest.Version = new ApiVersion(version);

            return this;
        }

        public IRequestEnvelope AgainstVersion(IApiVersion version)
        {
            _apiRequest.Version = version ?? throw new ArgumentNullException(nameof(version));

            return this;
        }

        public IRequestPerformer<TResponse> RespondsWith<TResponse>() where TResponse : class
        {
            _apiRequest.ResponseType = typeof(TResponse);

            return new SoapRequestFactory<TResponse>(_apiHandler, _apiRequest, _dependencies.GetRequiredDependency<IRouteFactory>());
        }

        public IEnvelopeFactory<TEnvelope> UsingEnvelope<TEnvelope>()
        {
            return new EnvelopeFactory<TEnvelope>(this, _apiRequest, _dependencies.GetRequiredDependency<ISoapSerializer>());
        }

        public IResponseType UsingEnvelope<TEnvelope>(TEnvelope envelope)
        {
            SoapEnvelope soapEnvelope = new SoapEnvelope
            {
                Body = envelope
            };

            ISoapSerializer serializer = _dependencies.GetRequiredDependency<ISoapSerializer>();

            _apiRequest.Content = serializer.Serialize(soapEnvelope);

            return this;
        }

        public IResponseType WithParameters(Dictionary<string, object> parameters)
        {
            IObjectSerializer objectSerializer = _dependencies.GetRequiredDependency<IObjectSerializer>();

            Dictionary<string, string> convertedParameters = objectSerializer.SerializeToDictionary(parameters);

            return WithParameters(convertedParameters);
        }

        public IResponseType WithParameters(Dictionary<string, string> parameters)
        {
            IEnvelopmentService envelopment = _dependencies.GetRequiredDependency<IEnvelopmentService>();

            _apiRequest.Content = envelopment.Envelop(parameters, _service, "ser", "http://services.acuritywebservices.finsyn.com.au/");

            return this;
        }

        public IResponseType WithParameters<TParam>(TParam parameters) where TParam : class
        {
            IObjectSerializer objectSerializer = _dependencies.GetRequiredDependency<IObjectSerializer>();

            Dictionary<string, string> convertedParameters = objectSerializer.SerializeToDictionary(parameters);

            return WithParameters(convertedParameters);
        }
    }
}