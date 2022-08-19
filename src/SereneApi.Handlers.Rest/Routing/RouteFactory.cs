using DeltaWare.SDK.SmartFormat;
using SereneApi.Core.Configuration.Handler;
using SereneApi.Core.Http;
using SereneApi.Handlers.Rest.Queries;
using SereneApi.Handlers.Rest.Requests;
using System;
using System.Linq;

namespace SereneApi.Handlers.Rest.Routing
{
    /// <inheritdoc cref="IRouteFactory"/>
    internal class RouteFactory : IRouteFactory
    {
        private readonly IQuerySerializer _querySerializer;

        private readonly IConnectionSettings _connectionSettings;

        private readonly string _routeTemplate;

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of <see cref="RouteFactory"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public RouteFactory(IQuerySerializer querySerializer, IConnectionSettings connectionSettings, HandlerConfiguration configuration)
        {
            _querySerializer = querySerializer;
            _connectionSettings = connectionSettings;
            _routeTemplate = configuration?.GetRouteTemplate() ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion Constructors

        public string BuildEndPoint(IRestApiRequest request)
        {
            if (request.Parameters != null)
            {
                if (!string.IsNullOrWhiteSpace(request.EndpointTemplate))
                {
                    return FormatEndpointTemplate(request.EndpointTemplate, request.Parameters);
                }

                if (request.Parameters.Length > 1)
                {
                    throw new ArgumentException("An endpoint template must be supplied to use multiple parameters.");
                }

                return request.Parameters.First().ToString();

            }

            if (!string.IsNullOrWhiteSpace(request.EndpointTemplate))
            {
                return request.EndpointTemplate;
            }

            return null;
        }

        /// <inheritdoc cref="IRouteFactory.BuildRoute"/>
        public Uri BuildRoute(IRestApiRequest request)
        {
            RestApiRequest apiRequest = (RestApiRequest)request;

            string query = null;

            if (apiRequest.Query is { Count: > 0 })
            {
                query = _querySerializer.Serialize(apiRequest.Query);
            }

            string route = SmartFormat.Parse(_routeTemplate, new
            {
                ResourcePath = request.ResourcePath?.TrimEnd('/'),
                Resource = request.Resource?.TrimEnd('/'),
                Version = request.Version?.GetVersionString()?.TrimEnd('/'),
                Endpoint = request.Endpoint?.TrimEnd('/'),
                Query = query
            });

            return new Uri(route.TrimStart('/'), UriKind.Relative);
        }

        public Uri GetUrl(IRestApiRequest apiRequest)
        {
            string url = $"{_connectionSettings.BaseAddress}{apiRequest.Route}";

            return new Uri(url);
        }

        /// <summary>
        /// Formats the End Point Template.
        /// </summary>
        /// <param name="endpointTemplate">The End Point Template to be formatted.</param>
        /// <param name="templateParameters">The Parameters to be appended to the template.</param>
        /// <exception cref="FormatException">Thrown when an incorrect end point is provided.</exception>
        private static string FormatEndpointTemplate(string endpointTemplate, params object[] templateParameters)
        {
            int expectedFormatLength = endpointTemplate.Length - templateParameters.Length * 3;

            foreach (object parameter in templateParameters)
            {
                expectedFormatLength += parameter.ToString().Length;
            }

            string endpoint = string.Format(endpointTemplate, templateParameters);

            if (endpoint != endpointTemplate && expectedFormatLength == endpoint.Length)
            {
                return $"{endpoint}";
            }

            if (templateParameters.Length > 1)
            {
                throw new FormatException("Multiple Parameters must be used with a format-table endpoint template.");
            }

            endpoint = endpointTemplate;

            return $"{endpoint}/{templateParameters.First()}";
        }
    }
}