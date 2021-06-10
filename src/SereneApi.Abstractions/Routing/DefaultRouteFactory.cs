using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Requests;
using System;
using System.Linq;

namespace SereneApi.Abstractions.Routing
{
    /// <inheritdoc cref="IRouteFactory"/>
    internal class DefaultRouteFactory : IRouteFactory
    {
        private readonly IDependencyProvider _dependencies;

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of <see cref="DefaultRouteFactory"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public DefaultRouteFactory(IDependencyProvider dependencies)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }

        #endregion

        public string BuildEndPoint(IApiRequest request)
        {
            if (request.Parameters != null)
            {
                // If parameters are not empty, the route will need to have them added.
                if (string.IsNullOrWhiteSpace(request.EndpointTemplate))
                {
                    // No Endpoint was supplied, if one parameter was provided it will be appended.
                    // Else an exception will be thrown as a template is needed if more than one parameter is provided.
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
        public Uri BuildRoute(IApiRequest request)
        {
            string route = $"{request.ResourcePath}{request.Resource}";

            if (!string.IsNullOrWhiteSpace(request.Endpoint))
            {
                route += $"/{request.Endpoint}";
            }

            if (request.Query != null && request.Query.Count > 0)
            {
                IQueryFactory queryFactory = _dependencies.GetDependency<IQueryFactory>();

                string queryString = queryFactory.Build(request.Query);

                // If a query is provided it will be appended.
                route += queryString;
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

            // This should not need to be done, but if it is not done a format that only support 1 parameter but is supplied more than 1 parameter will not fail.
            int expectedFormatLength = endpointTemplate.Length - templateParameters.Length * 3;

            foreach (object parameter in templateParameters)
            {
                expectedFormatLength += parameter.ToString().Length;
            }

            #endregion

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

            // Return an endpoint without formatting the template and appending the only parameter to the end.
            return $"{endpoint}/{templateParameters.First()}";
        }
    }
}
