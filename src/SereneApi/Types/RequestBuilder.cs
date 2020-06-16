using SereneApi.Helpers;
using SereneApi.Interfaces;
using System;
using System.Linq.Expressions;

namespace SereneApi.Types
{
    public class RequestBuilder: IRequest
    {
        #region Readonly Variables

        private readonly IRouteFactory _routeFactory;

        private readonly IQueryFactory _queryFactory;

        private readonly ISerializer _serializer;

        private readonly string _resource;

        #endregion

        private Method _method;

        private IApiRequestContent _requestContent;

        private string _endPoint;

        private object[] _endPointParameters;

        private string _query;

        private string _suppliedResource;


        public RequestBuilder(IRouteFactory routeFactory, IQueryFactory queryFactory, ISerializer serializer, string resource = null)
        {
            _routeFactory = routeFactory;
            _queryFactory = queryFactory;
            _serializer = serializer;
            _resource = resource;
        }

        public void UsingMethod(Method method)
        {
            if(method == Method.None)
            {
                throw new ArgumentException("Must use a valid Method.", nameof(method));
            }

            _method = method;
        }

        public IRequestEndPoint AgainstResource(string resource)
        {
            if(_resource != null)
            {
                throw new MethodAccessException("This method can only be called if a Resource was not provided.");
            }

            _suppliedResource = resource;

            return this;
        }

        public IRequestCreated WithInBodyContent<TContent>(TContent content)
        {
            ExceptionHelper.EnsureParameterIsNotNull(content, nameof(content));

            _requestContent = _serializer.Serialize(content);

            return this;
        }

        public IRequestCreated WithQuery<TQueryable>(TQueryable queryable)
        {
            ExceptionHelper.EnsureParameterIsNotNull(queryable, nameof(queryable));

            _query = _queryFactory.Build(queryable);

            return this;
        }

        public IRequestCreated WithQuery<TQueryable>(TQueryable queryable, Expression<Func<TQueryable, object>> queryExpression)
        {
            ExceptionHelper.EnsureParameterIsNotNull(queryable, nameof(queryable));
            ExceptionHelper.EnsureParameterIsNotNull(queryExpression, nameof(queryExpression));

            _query = _queryFactory.Build(queryable, queryExpression);

            return this;
        }

        public IRequestContent WithEndPoint(object parameter = null)
        {
            _endPoint = parameter?.ToString();

            return this;
        }

        public IRequestContent WithEndPoint(string endPoint)
        {
            ExceptionHelper.EnsureParameterIsNotNull(endPoint, nameof(endPoint));

            _endPoint = endPoint;

            return this;
        }

        public IRequestContent WithEndPointTemplate(string template, params object[] parameters)
        {
            ExceptionHelper.EnsureParameterIsNotNull(template, nameof(template));
            ExceptionHelper.EnsureArrayIsNotEmpty(parameters, nameof(parameters));

            _endPoint = template;
            _endPointParameters = parameters;

            return this;
        }

        public IApiRequest GetRequest()
        {
            if(_resource != null)
            {
                _routeFactory.WithResource(_resource);
            }
            else if(_suppliedResource != null)
            {
                _routeFactory.WithResource(_suppliedResource);
            }

            _routeFactory.AddQuery(_query);
            _routeFactory.AddEndPoint(_endPoint);
            _routeFactory.AddParameters(_endPointParameters);

            Uri endPoint = _routeFactory.BuildRoute();

            return new ApiRequest(_method, endPoint, _requestContent);
        }
    }
}
