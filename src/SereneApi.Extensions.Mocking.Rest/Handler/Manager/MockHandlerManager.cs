using DeltaWare.Dependencies.Abstractions;
using DeltaWare.SDK.Serialization.Types;
using DeltaWare.SDK.Serialization.Types.Attributes;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Http.Content.Types;
using SereneApi.Core.Serialization;
using SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors;
using SereneApi.Extensions.Mocking.Rest.Configuration.Settings;
using SereneApi.Extensions.Mocking.Rest.Exceptions;
using SereneApi.Extensions.Mocking.Rest.Handler.Attributes;
using SereneApi.Extensions.Mocking.Rest.Helpers;
using SereneApi.Extensions.Mocking.Rest.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Manager
{
    internal class MockHandlerManager : IMockHandlerManager
    {
        private readonly IDependencyProvider _dependencyProvider;

        private readonly ISerializer _serializer;

        private readonly IObjectSerializer _objectSerializer;

        private readonly ILogger _logger;

        public MockHandlerManager(IDependencyProvider dependencyProvider, ISerializer serializer, IObjectSerializer objectSerializer, ILogger logger = null)
        {
            _dependencyProvider = dependencyProvider;

            _serializer = serializer;
            _objectSerializer = objectSerializer;

            _logger = logger;
        }

        public async Task<IMockResponse> InvokeHandlerAsync(IMockHandlerDescriptor mockHandler, HttpRequestMessage request, string requestEndpoint)
        {
            object handlerInstance = _dependencyProvider.CreateInstance(mockHandler.HandlerType);

            object[] parameters = await BuildParametersAsync(mockHandler, request, requestEndpoint.Split('/'));

            object methodReturn = mockHandler.Method.Invoke(handlerInstance, parameters);

            if (methodReturn == null)
            {
                return null;
            }

            return await GetResponseAsync(methodReturn, mockHandler);
        }

        private async Task<object[]> BuildParametersAsync(IMockHandlerDescriptor mockHandler, HttpRequestMessage request, string[] requestEndpointSections)
        {
            ParameterInfo[] methodParameters = mockHandler.Method.GetParameters();

            object[] parameters = new object[methodParameters.Length];

            if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put || request.Method == HttpMethod.Patch)
            {
                int index = GetFromBodyParameterIndex(methodParameters);

                if (index != -1 && request.Content != null)
                {
                    parameters[index] = await _serializer.DeserializeAsync(methodParameters[index].ParameterType, new HttpContentResponse(request.Content));
                }
            }

            if (requestEndpointSections.Length == 0)
            {
                return parameters;
            }

            int queryIndex = GetFromQueryParameterIndex(methodParameters);

            if (queryIndex != -1)
            {
                parameters[queryIndex] = BuildQueryObject(methodParameters[queryIndex].ParameterType, requestEndpointSections.Last().Split('?').Last());
            }

            BuildEndpointParameters(methodParameters, mockHandler.MockMethod, requestEndpointSections, parameters);

            return parameters;
        }

        private void BuildEndpointParameters(ParameterInfo[] methodParameters, MockMethodAttribute method, string[] requestEndpointSections, object[] parameters)
        {
            string[] templateSections = method.EndpointTemplate.Split('/');

            for (int i = 0; i < templateSections.Length; i++)
            {
                string templateSection = templateSections[i];

                if (!ParameterHelper.IsParameter(templateSection))
                {
                    continue;
                }

                string parameterName = ParameterHelper.GetParameterKey(templateSection);

                bool optional = false;

                if (ParameterHelper.IsParameterOptional(parameterName))
                {
                    optional = true;

                    parameterName = ParameterHelper.RemoveOptionalKey(parameterName);
                }

                if (requestEndpointSections.Length - 1 < i)
                {
                    if (optional)
                    {
                        continue;
                    }

                    throw new EndpointMismatchException(requestEndpointSections, templateSections);
                }

                string value = requestEndpointSections[i];

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(parameterName);
                }

                int parameterIndex = Array.FindIndex(methodParameters, info => info.Name == parameterName);

                if (parameterIndex == -1)
                {
                    throw new TemplateParameterNotFoundException(parameterName);
                }

                parameters[parameterIndex] = _objectSerializer.Deserialize(value, methodParameters[parameterIndex].ParameterType);
            }
        }

        private async Task<IMockResponse> GetResponseAsync(object methodReturn, IMockHandlerDescriptor mockHandler)
        {
            IMockResult mockResult;

            if (methodReturn is IMockResult result)
            {
                mockResult = result;
            }
            else if (methodReturn is Task<IMockResult> mockResultTask)
            {
                // await the response.
                mockResult = await mockResultTask;
            }
            else
            {
                throw new InvalidMockRestApiHandlerReturnTypeException(mockHandler, methodReturn.GetType());
            }

            MockResponse response = new MockResponse
            {
                Status = mockResult.Status
            };

            if (TryGetDelaySettings(mockHandler, out IDelaySettings delaySettings))
            {
                response.Delay = delaySettings;
            }

            if (mockResult is MockObjectResult objectResult)
            {
                response.Content = _serializer.Serialize(objectResult.Result);
            }

            return response;
        }

        private bool TryGetDelaySettings(IMockHandlerDescriptor mockHandler, out IDelaySettings delaySettings)
        {
            DelayedAttribute delay = mockHandler.Method.GetCustomAttribute<DelayedAttribute>(true);

            if (delay != null)
            {
                delaySettings = delay.GetDelaySettings();

                return true;
            }

            delay = mockHandler.HandlerType.GetCustomAttribute<DelayedAttribute>(true);

            if (delay != null)
            {
                delaySettings = delay.GetDelaySettings();

                return true;
            }

            delaySettings = null;

            return false;
        }

        private int GetFromBodyParameterIndex(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].GetCustomAttribute<BindBodyAttribute>() == null)
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        private int GetFromQueryParameterIndex(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].GetCustomAttribute<BindQueryAttribute>() == null)
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        private object BuildQueryObject(Type type, string queryString)
        {
            Dictionary<string, string> queryDictionary = BuildQueryDictionary(queryString);

            object queryObject = Activator.CreateInstance(type);

            foreach (PropertyInfo property in type.GetPublicProperties())
            {
                NameAttribute nameAttribute = property.GetCustomAttribute<NameAttribute>();

                string name = property.Name;

                if (nameAttribute != null)
                {
                    name = nameAttribute.Value;
                }

                if (!queryDictionary.TryGetValue(name, out string value))
                {
                    continue;
                }

                object propertyValue = _objectSerializer.Deserialize(value, property);

                property.SetValue(queryObject, propertyValue);
            }

            return queryObject;
        }

        private Dictionary<string, string> BuildQueryDictionary(string queryString)
        {
            return queryString
                .Split('&')
                .Select(querySection => querySection.Split('='))
                .ToDictionary(queryKvp => queryKvp[0], queryKvp => queryKvp[1]);
        }
    }
}
