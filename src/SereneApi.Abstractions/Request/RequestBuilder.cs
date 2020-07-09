using DeltaWare.Dependencies;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Helpers;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Serializers;
using System;
using System.Linq.Expressions;

namespace SereneApi.Abstractions.Request
{
    /// <summary>
    ///  Builds an <see cref="IApiRequest"/> based on the supplied values.
    /// </summary>
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

        public RequestBuilder(IDependencyProvider dependencies, string resource = null)
        {
            _routeFactory = dependencies.GetDependency<IRouteFactory>();
            _queryFactory = dependencies.GetDependency<IQueryFactory>();
            _serializer = dependencies.GetDependency<ISerializer>();
            _resource = resource;
        }

        public void UsingMethod(Method method)
        {
            if(method == Method.NONE)
            {
                throw new ArgumentException("Must use a valid Method.", nameof(method));
            }

            _method = method;
        }

        /// <inheritdoc cref="IRequest.AgainstResource"/>
        public IRequestEndPoint AgainstResource(string resource)
        {
            if(_resource != null)
            {
                throw new MethodAccessException("This method can only be called if a Resource was not provided.");
            }

            _suppliedResource = resource;

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IRequestContent.WithInBodyContent</cref>
        /// </inheritdoc>
        public IRequestCreated WithInBodyContent<TContent>(TContent content)
        {
            ExceptionHelper.EnsureParameterIsNotNull(content, nameof(content));

            _requestContent = _serializer.Serialize(content);

            return this;
        }

        public IRequestCreated WithInBodyContent(IApiRequestContent content)
        {
            ExceptionHelper.EnsureParameterIsNotNull(content, nameof(content));

            _requestContent = content;

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IRequestContent.WithQuery</cref>
        /// </inheritdoc>
        public IRequestCreated WithQuery<TQueryable>(TQueryable queryable)
        {
            ExceptionHelper.EnsureParameterIsNotNull(queryable, nameof(queryable));

            _query = _queryFactory.Build(queryable);

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IRequestContent.WithQuery</cref>
        /// </inheritdoc>
        public IRequestCreated WithQuery<TQueryable>(TQueryable queryable,
            Expression<Func<TQueryable, object>> queryExpression)
        {
            ExceptionHelper.EnsureParameterIsNotNull(queryable, nameof(queryable));
            ExceptionHelper.EnsureParameterIsNotNull(queryExpression, nameof(queryExpression));

            _query = _queryFactory.Build(queryable, queryExpression);

            return this;
        }

        /// <see>
        ///     <cref>IRequestEndPoint.WithEndPoint</cref>
        /// </see>
        public IRequestContent WithEndPoint(object parameter)
        {
            ExceptionHelper.EnsureParameterIsNotNull(parameter, nameof(parameter));

            _endPoint = parameter.ToString();

            return this;
        }

        /// <see>
        ///     <cref>IRequestEndPoint.WithEndPoint</cref>
        /// </see>
        public IRequestContent WithEndPoint(string endPoint)
        {
            ExceptionHelper.EnsureParameterIsNotNull(endPoint, nameof(endPoint));

            _endPoint = endPoint;

            return this;
        }

        /// <see cref="IRequestEndPoint.WithEndPointTemplate"/>
        public IRequestContent WithEndPointTemplate(string template, params object[] parameters)
        {
            ExceptionHelper.EnsureParameterIsNotNull(template, nameof(template));
            ExceptionHelper.EnsureArrayIsNotEmpty(parameters, nameof(parameters));

            _endPoint = template;
            _endPointParameters = parameters;

            return this;
        }

        /// <see cref="IRequestCreated.GetRequest"/>
        public IApiRequest GetRequest()
        {
            if(_resource != null)
            {
                _routeFactory.AddResource(_resource);
            }
            else if(_suppliedResource != null)
            {
                _routeFactory.AddResource(_suppliedResource);
            }

            if(!string.IsNullOrWhiteSpace(_endPoint))
            {
                _routeFactory.AddEndPoint(_endPoint);
            }

            if(_endPointParameters != null)
            {
                _routeFactory.AddParameters(_endPointParameters);
            }

            if(!string.IsNullOrWhiteSpace(_query))
            {
                _routeFactory.AddQuery(_query);
            }

            Uri endPoint = _routeFactory.BuildRoute();

            return new ApiRequest(_method, endPoint, _requestContent);
        }
    }
}
