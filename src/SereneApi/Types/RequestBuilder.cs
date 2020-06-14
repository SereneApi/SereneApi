using SereneApi.Helpers;
using SereneApi.Interfaces;
using System;
using System.Linq.Expressions;

namespace SereneApi.Types
{
    public class RequestBuilder : IRequest
    {
        #region Readonly Variables

        private readonly IRouteFactory _routeFactory;

        private readonly IQueryFactory _queryFactory;

        private readonly ISerializer _serializer;

        #endregion

        private Method _method;

        private IApiRequestContent _requestContent;

        private string _endPoint;

        private object[] _endPointParameters;

        private string _query;

        public RequestBuilder(IRouteFactory routeFactory, IQueryFactory queryFactory, ISerializer serializer)
        {
            _routeFactory = routeFactory;
            _queryFactory = queryFactory;
            _serializer = serializer;
        }

        public void UsingMethod(Method method)
        {
            if (method == Method.None)
            {
                throw new ArgumentException("Must use a valid Method.", nameof(method));
            }

            _method = method;
        }

        public IRequestCreated AddInBodyContent<TContent>(TContent content)
        {
            ExceptionHelper.CheckParameterIsNull(content, nameof(content));

            _requestContent = _serializer.Serialize(content);

            return this;
        }

        public IRequestCreated AddQuery<TQueryable>(TQueryable queryable)
        {
            ExceptionHelper.CheckParameterIsNull(queryable, nameof(queryable));

            _query = _queryFactory.Build(queryable);

            return this;
        }

        public IRequestCreated AddQuery<TQueryable>(TQueryable queryable, Expression<Func<TQueryable, object>> queryExpression)
        {
            ExceptionHelper.CheckParameterIsNull(queryable, nameof(queryable));
            ExceptionHelper.CheckParameterIsNull(queryExpression, nameof(queryExpression));

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
            ExceptionHelper.CheckParameterIsNull(endPoint, nameof(endPoint));

            _endPoint = endPoint;

            return this;
        }

        public IRequestContent WithEndPoint(string endPointTemplate, params object[] templateParameters)
        {
            ExceptionHelper.CheckParameterIsNull(endPointTemplate, nameof(endPointTemplate));
            ExceptionHelper.CheckArrayIsEmpty(templateParameters, nameof(templateParameters));

            _endPoint = endPointTemplate;
            _endPointParameters = templateParameters;

            return this;
        }

        public IApiRequest GetRequest()
        {
            _routeFactory.AddQuery(_query);
            _routeFactory.AddEndpoint(_endPoint);
            _routeFactory.AddParameters(_endPointParameters);

            Uri endPoint = _routeFactory.BuildRoute();

            return new ApiRequest(_method, endPoint, _requestContent);
        }
    }
}
