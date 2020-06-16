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

        private string _endPoint;

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

        public void AddEndPoint(string endPoint)
        {
            if(!string.IsNullOrWhiteSpace(_endPoint))
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            _endPoint = endPoint;
        }

        public Uri BuildRoute()
        {
            string route = $"{ResourcePath}{_resource}";

            if(_parameters != null)
            {
                if(string.IsNullOrWhiteSpace(_endPoint))
                {
                    if(_parameters.Length > 1)
                    {
                        throw new ArgumentException("An endPoint template must be supplied to use multiple parameters.");
                    }

                    route += _parameters.First();
                }
                else
                {
                    string template = FormatEndPointTemplate(_endPoint, _parameters);

                    route += $"/{template}";
                }
            }
            else if(!string.IsNullOrWhiteSpace(_endPoint))
            {
                route += $"/{_endPoint}";
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
            _endPoint = null;
            _parameters = null;
            _query = null;
            _resource = null;
        }

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
            return $"{endPoint}/{templateParameters[0]}";
        }
    }
}
