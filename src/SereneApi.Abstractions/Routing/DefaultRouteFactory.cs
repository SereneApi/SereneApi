using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Abstractions.Routing
{
    /// <inheritdoc cref="IRouteFactory"/>
    internal class DefaultRouteFactory: IRouteFactory
    {
        #region Variables

        private string _query;

        private object[] _parameters;

        private string _endpoint;

        private string _resource;

        #endregion
        #region Properties

        /// <inheritdoc cref="IRouteFactory.ResourcePath"/>
        public string ResourcePath { get; }

        #endregion
        #region Constructors

        /// <summary>
        /// Instantiates a new instance of <see cref="DefaultRouteFactory"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public DefaultRouteFactory(IConnectionConfiguration connection)
        {
            if(connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            ResourcePath = connection.ResourcePath;
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="DefaultRouteFactory"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public DefaultRouteFactory([NotNull] IDependencyProvider provider)
        {
            if(provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            ResourcePath = provider.GetDependency<IConnectionConfiguration>().ResourcePath;
        }

        #endregion

        /// <inheritdoc cref="IRouteFactory.AddResource"/>
        public void AddResource([NotNull] string resource)
        {
            if(string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentNullException(nameof(resource));
            }

            _resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
        }

        /// <inheritdoc cref="IRouteFactory.AddQuery"/>
        public void AddQuery([NotNull] string queryString)
        {
            if(string.IsNullOrWhiteSpace(queryString))
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            _query = queryString;
        }

        /// <inheritdoc cref="IRouteFactory.AddParameters"/>
        public void AddParameters([NotNull] params object[] parameters)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <inheritdoc cref="IRouteFactory.AddEndpoint"/>
        public void AddEndpoint([NotNull] string endpoint)
        {
            if(string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            _endpoint = SourceHelpers.EnsureSourceNoSlashTermination(endpoint);
        }

        /// <inheritdoc cref="IRouteFactory.BuildRoute"/>
        public Uri BuildRoute(bool clearSettings = true)
        {
            string route = $"{ResourcePath}{_resource}";

            if(_parameters != null)
            {
                // If parameters are not empty, the route will need to have them added.
                if(string.IsNullOrWhiteSpace(_endpoint))
                {
                    // No Endpoint was supplied, if one parameter was provided it will be appended.
                    // Else an exception will be thrown as a template is needed if more than one parameter is provided.
                    if(_parameters.Length > 1)
                    {
                        throw new ArgumentException("An endpoint template must be supplied to use multiple parameters.");
                    }

                    route += "/" + _parameters.First();
                }
                else
                {
                    // An Endpoint was provided so it will be formatted.
                    string template = FormatEndpointTemplate(_endpoint, _parameters);

                    route += $"/{template}";
                }
            }
            else if(!string.IsNullOrWhiteSpace(_endpoint))
            {
                // No parameter was provided so only the endpoint is appended.
                route += $"/{_endpoint}";
            }

            if(!string.IsNullOrWhiteSpace(_query))
            {
                // If a query is provided it will be appended.
                route += _query;
            }

            if(clearSettings)
            {
                // Clear the values as the route has been built.
                Clear();
            }

            return new Uri(route, UriKind.Relative);
        }

        /// <inheritdoc cref="IRouteFactory.Clear"/>
        public void Clear()
        {
            _endpoint = null;
            _parameters = null;
            _query = null;
            _resource = null;
        }

        /// <summary>
        /// Formats the End Point Template.
        /// </summary>
        /// <param name="endpointTemplate">The End Point Template to be formatted.</param>
        /// <param name="templateParameters">The Parameters to be appended to the template.</param>
        /// <exception cref="FormatException">Thrown when an incorrect end point is provided.</exception>
        private static string FormatEndpointTemplate(string endpointTemplate, params object[] templateParameters)
        {
            #region Format Check Logic

            // This should not need to be done, but if it is not done a format that only support 1 parameter but is supplied more than 1 parameter will not fail.
            int expectedFormatLength = endpointTemplate.Length - templateParameters.Length * 3;

            foreach(object parameter in templateParameters)
            {
                expectedFormatLength += parameter.ToString().Length;
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
            return $"{endpoint}/{templateParameters.First()}";
        }
    }
}
