using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Transformation;
using SereneApi.Core.Versioning;
using SereneApi.Handlers.Soap.Extensions;
using SereneApi.Handlers.Soap.Requests.Types;
using System;
using System.Collections.Generic;

namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public class ApiRequestFactory : IApiRequestFactory, IApiRequestParameters
    {
        private readonly IDependencyProvider _dependencies;

        private readonly SoapApiHandler _apiHandler;

        private readonly SoapApiRequest _apiRequest;

        public ApiRequestFactory(SoapApiHandler apiHandler)
        {
            _apiHandler = apiHandler;
            _apiRequest = apiHandler.Connection.GenerateApiRequest();

            _dependencies = apiHandler.Options.Dependencies;
        }

        public IApiRequestParameters AgainstService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            _apiRequest.Service = serviceName;

            return this;
        }

        public IApiResponseType WithParameters(Dictionary<string, object> parameters)
        {
            Dictionary<string, string> convertedParameters = new();

            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                convertedParameters.Add(parameter.Key, parameter.ToString());
            }

            _apiRequest.Parameters = convertedParameters;

            return this;
        }

        public IApiResponseType WithParameters<TParam>(TParam parameters) where TParam : class
        {
            ITransformationService transformation = _dependencies.GetDependency<ITransformationService>();

            _apiRequest.Parameters = transformation.BuildDictionary(parameters);

            return this;
        }

        public IApiRequestPerformer<TResponse> RespondsWith<TResponse>() where TResponse : class
        {
            _apiRequest.ResponseType = typeof(TResponse);

            return new ApiRequestFactory<TResponse>(_apiHandler, _apiRequest);
        }

        public IApiRequestService AgainstVersion(string version)
        {
            throw new NotImplementedException();
        }

        public IApiRequestService AgainstVersion(IApiVersion version)
        {
            throw new NotImplementedException();
        }
    }
}
