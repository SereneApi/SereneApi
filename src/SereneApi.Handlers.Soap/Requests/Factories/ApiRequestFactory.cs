using System;
using System.Collections.Generic;
using System.Reflection;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Handlers.Soap.Extensions;
using SereneApi.Handlers.Soap.Requests.Types;

namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public class ApiRequestFactory : IApiRequestFactory, IApiRequestParameters, IApiResponseType
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

        public IApiRequestPerformer<TResponds> RespondsWith<TResponds>() where TResponds : class
        {
            throw new System.NotImplementedException();
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
            PropertyInfo[] properties = typeof(TParam).GetProperties();

            Dictionary<string, string> convertedParameters = new();

            foreach (PropertyInfo property in properties)
            {
                property.GetValue()
            }

            throw new NotImplementedException();
        }
    }
}
