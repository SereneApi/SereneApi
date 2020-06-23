using SereneApi.Helpers;
using SereneApi.Interfaces;
using System;
using System.Linq;

namespace SereneApi.Factories
{
    /// <inheritdoc cref="IRouteFactory"/>
    public sealed class RouteFactory: IRouteFactory
    {
        #region Variables

        private string _query;

        private object[] _parameters;

        private string _endPoint;

        private string _resource;

        #endregion
        #region Properties

        /// <inheritdoc cref="IRouteFactory.ResourcePath"/>
        public string ResourcePath { get; }

        #endregion
        #region Constructors

        /// <summary>
        /// Instantiates a new instance of <see cref="RouteFactory"/> with an empty Resource Path.
        /// </summary>
        public RouteFactory()
        {
            ResourcePath = null;
        }

        /// <summary>
        /// Instantiates a new instance of <see cref="RouteFactory"/>.
        /// </summary>
        /// <param name="connectionInfo"></param>
        public RouteFactory(IConnectionInfo connectionInfo)
        {
            ResourcePath = connectionInfo.ResourcePath;
        }

        #endregion

        /// <inheritdoc cref="IRouteFactory.AddResource"/>
        public void AddResource(string resource)
        {
            ExceptionHelper.EnsureParameterIsNotNull(resource, nameof(resource));

            _resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);
        }

        /// <inheritdoc cref="IRouteFactory.AddQuery"/>
        public void AddQuery(string queryString)
        {
            if(!string.IsNullOrWhiteSpace(_query))
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            ExceptionHelper.EnsureParameterIsNotNull(queryString, nameof(queryString));

            _query = queryString;
        }

        /// <inheritdoc cref="IRouteFactory.AddParameters"/>
        public void AddParameters(params object[] parameters)
        {
            if(_parameters != null)
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            _parameters = parameters;
        }

        /// <inheritdoc cref="IRouteFactory.AddEndPoint"/>
        public void AddEndPoint(string endPoint)
        {
            if(!string.IsNullOrWhiteSpace(_endPoint))
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            ExceptionHelper.EnsureParameterIsNotNull(endPoint, nameof(endPoint));

            _endPoint = SourceHelpers.EnsureSourceNoSlashTermination(endPoint);
        }

        /// <inheritdoc cref="IRouteFactory.BuildRoute"/>
        public Uri BuildRoute()
        {
            string route = $"{ResourcePath}{_resource}";

            if(_parameters != null)
            {
                // If parameters are not empty, the route will need to have them added.
                if(string.IsNullOrWhiteSpace(_endPoint))
                {
                    // No Endpoint was supplied, if one parameter was provided it will be appended.
                    // Else an exception will be thrown as a template is needed if more than one parameter is provided.
                    if(_parameters.Length > 1)
                    {
                        throw new ArgumentException("An endPoint template must be supplied to use multiple parameters.");
                    }

                    route += "/" + _parameters.First();
                }
                else
                {
                    // An Endpoint was provided so it will be formatted.
                    string template = FormatEndPointTemplate(_endPoint, _parameters);

                    route += $"/{template}";
                }
            }
            else if(!string.IsNullOrWhiteSpace(_endPoint))
            {
                // No parameter was provided so only the endpoint is appended.
                route += $"/{_endPoint}";
            }

            if(!string.IsNullOrWhiteSpace(_query))
            {
                // If a query is provided it will be appended.
                route += _query;
            }

            // Clear the values as the route has been built.
            Clear();

            return new Uri(route, UriKind.Relative);
        }

        /// <inheritdoc cref="IRouteFactory.Clear"/>
        public void Clear()
        {
            _endPoint = null;
            _parameters = null;
            _query = null;
            _resource = null;
        }

        /// <summary>
        /// Formats the End Point Template.
        /// </summary>
        /// <param name="endPointTemplate">The End Point Template to be formmatted.</param>
        /// <param name="templateParameters">The Parameters to be appended to the template.</param>
        /// <returns></returns>
        private static string FormatEndPointTemplate(string endPointTemplate, params object[] templateParameters)
        {
            #region Format Check Logic

            // This should not need to be done, but if it is not done a format that only support 1 parameter but is supplied more than 1 parameter will not fail.
            int expectedFormatLength = endPointTemplate.Length - templateParameters.Length * 3;

            for(int i = 0; i < templateParameters.Length; i++)
            {
                expectedFormatLength += templateParameters[i].ToString().Length;
            }

            #endregion

            string endPoint = string.Format(endPointTemplate, templateParameters);

            // If the length is different the endPoint has been formatted correctly.
            if(endPoint != endPointTemplate && expectedFormatLength == endPoint.Length)
            {
                return $"{endPoint}";
            }

            // If we have more than 1 parameter here it means the formatting was unsuccessful.
            if(templateParameters.Length > 1)
            {
                throw new FormatException("Multiple Parameters must be used with a format-table endPoint template.");
            }

            endPoint = endPointTemplate;

            // Return an endPoint without formatting the template and appending the only parameter to the end.
            return $"{endPoint}/{templateParameters.First()}";
        }
    }
}
