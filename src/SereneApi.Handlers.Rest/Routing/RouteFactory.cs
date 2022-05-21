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

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of <see cref="RouteFactory"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public RouteFactory(IQuerySerializer querySerializer)
        {
            _querySerializer = querySerializer ?? throw new ArgumentNullException(nameof(querySerializer));
        }

        #endregion Constructors

        public string BuildEndPoint(IRestApiRequest request)
        {
            if (request.Parameters != null)
            {
                // If parameters are not empty, the route will need to have them added.
                if (string.IsNullOrWhiteSpace(request.EndpointTemplate))
                {
                    // No Endpoint was supplied, if one parameter was provided it will be appended.
                    // Else an exception will be thrown as a template is needed if more than one
                    // parameter is provided.
                    if (request.Parameters.Length > 1)
                    {
                        throw new ArgumentException("An endpoint template must be supplied to use multiple parameters.");
                    }

                    return request.Parameters.First().ToString();
                }
                // An Endpoint was provided so it will be formatted.
                return FormatEndpointTemplate(request.EndpointTemplate, request.Parameters);
            }

            if (!string.IsNullOrWhiteSpace(request.EndpointTemplate))
            {
                // No parameter was provided so only the endpoint is appended.
                return request.EndpointTemplate;
            }

            return null;
        }

        /// <inheritdoc cref="IRouteFactory.BuildRoute"/>
        public Uri BuildRoute(IRestApiRequest request)
        {
            RestApiRequest apiRequest = (RestApiRequest)request;

            string route = string.Empty;

            if (!string.IsNullOrWhiteSpace(request.ResourcePath))
            {
                route += request.ResourcePath;
            }

            if (!string.IsNullOrWhiteSpace(request.Resource))
            {
                if (route.Length > 0 && route.Last() != '/')
                {
                    route += '/';
                }

                route += request.Resource;
            }

            if (request.Version != null)
            {
                if (route.Length > 0 && route.Last() != '/')
                {
                    route += '/';
                }

                route += request.Version.GetVersionString();
            }

            if (!string.IsNullOrWhiteSpace(request.Endpoint))
            {
                if (route.Length > 0 && route.Last() != '/')
                {
                    route += '/';
                }

                route += request.Endpoint;
            }

            if (apiRequest.Query is { Count: > 0 })
            {
                route += _querySerializer.Serialize(apiRequest.Query);
            }

            return new Uri(route, UriKind.Relative);
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

            // This should not need to be done, but if it is not done a format that only supports 1
            // parameter but is supplied more than 1 parameter will not fail.
            int expectedFormatLength = endpointTemplate.Length - templateParameters.Length * 3;

            foreach (object parameter in templateParameters)
            {
                expectedFormatLength += parameter.ToString().Length;
            }

            #endregion Format Check Logic

            string endpoint = string.Format(endpointTemplate, templateParameters);

            // If the length is different the endpoint has been formatted correctly.
            if (endpoint != endpointTemplate && expectedFormatLength == endpoint.Length)
            {
                return $"{endpoint}";
            }

            // If we have more than 1 parameter here it means the formatting was unsuccessful.
            if (templateParameters.Length > 1)
            {
                throw new FormatException("Multiple Parameters must be used with a format-table endpoint template.");
            }

            endpoint = endpointTemplate;

            // Return an endpoint without formatting the template and appending the only parameter
            // to the end.
            return $"{endpoint}/{templateParameters.First()}";
        }
    }
}