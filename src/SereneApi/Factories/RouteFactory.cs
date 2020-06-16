using SereneApi.Helpers;
using SereneApi.Interfaces;
using System;
using System.Linq;

namespace SereneApi.Factories
{
    public sealed class RouteFactory: IRouteFactory
    {
        #region Variables

        private string _query;

        private object[] _parameters;

        private string _endpoint;

        private string _resource;

        #endregion
        #region Properties

        public string ResourcePath { get; }

        #endregion
        #region Constructors

        public RouteFactory()
        {
            ResourcePath = string.Empty;
        }

        public RouteFactory(string resourcePath)
        {
            ResourcePath = resourcePath;
        }

        #endregion

        public void WithResource(string resource)
        {
            ExceptionHelper.EnsureParameterIsNotNull(resource, nameof(resource));

            _resource = resource;
        }

        public void AddQuery(string queryString)
        {
            if(!string.IsNullOrWhiteSpace(_query))
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            _query = queryString;
        }

        public void AddParameters(params object[] parameters)
        {
            if(_parameters != null)
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            _parameters = parameters;
        }

        public void AddEndpoint(string endpoint)
        {
            if(!string.IsNullOrWhiteSpace(_endpoint))
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            _endpoint = endpoint;
        }

        public Uri BuildRoute()
        {
            string route = $"{ResourcePath}{_resource}";

            if(_parameters != null)
            {
                if(string.IsNullOrWhiteSpace(_endpoint))
                {
                    if(_parameters.Length > 1)
                    {
                        throw new ArgumentException("An endpoint template must be supplied to use multiple parameters.");
                    }

                    route += _parameters.First();
                }
                else
                {
                    string template = FormatEndpointTemplate(_endpoint, _parameters);

                    route += $"/{template}";
                }
            }
            else if(!string.IsNullOrWhiteSpace(_endpoint))
            {
                route += $"/{_endpoint}";
            }

            if(!string.IsNullOrWhiteSpace(_query))
            {
                route += _query;
            }

            Clear();

            return new Uri(route, UriKind.Relative);
        }

        public void Clear()
        {
            _endpoint = null;
            _parameters = null;
            _query = null;
            _resource = null;
        }

        private static string FormatEndpointTemplate(string endpointTemplate, params object[] templateParameters)
        {
            #region Format Check Logic

            // This should not need to be done, but if it is not done a format that only support 1 parameter but is supplied more than 1 parameter will not fail.
            int expectedFormatLength = endpointTemplate.Length - templateParameters.Length * 3;

            for(int i = 0; i < templateParameters.Length; i++)
            {
                expectedFormatLength += templateParameters[i].ToString().Length;
            }

            #endregion

            string endpoint = string.Format(endpointTemplate, templateParameters);

            // If the length is different the endpoint has been formatted correctly.
            if(endpoint != endpointTemplate && expectedFormatLength == endpoint.Length)
            {
                return $"{endpoint}";
            }

            // If we have more than 1 parameter here it means the formatting was unsuccessful.
            if(templateParameters.Length > 1)
            {
                throw new FormatException("Multiple Parameters must be used with a format-table endpoint template.");
            }

            endpoint = endpointTemplate;

            // Return an endpoint without formatting the template and appending the only parameter to the end.
            return $"{endpoint}/{templateParameters[0]}";
        }
    }
}
