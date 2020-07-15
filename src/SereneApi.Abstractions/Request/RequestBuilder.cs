using DeltaWare.Dependencies;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Routing;
using SereneApi.Abstractions.Serializers;
using System;
using System.Diagnostics.CodeAnalysis;
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

        private string _endpoint;

        private object[] _endpointParameters;

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
        public IRequestEndpoint AgainstResource([NotNull] string resource)
        {
            _suppliedResource = resource ?? throw new ArgumentNullException(nameof(resource));

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IRequestContent.WithInBodyContent</cref>
        /// </inheritdoc>
        public IRequestCreated WithInBodyContent<TContent>([NotNull] TContent content)
        {
            if(content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            _requestContent = _serializer.Serialize(content);

            return this;
        }

        public IRequestCreated WithInBodyContent([NotNull] IApiRequestContent content)
        {
            _requestContent = content ?? throw new ArgumentNullException(nameof(content));

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IRequestContent.WithQuery</cref>
        /// </inheritdoc>
        public IRequestCreated WithQuery<TQueryable>([NotNull] TQueryable queryable)
        {
            if(queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            _query = _queryFactory.Build(queryable);

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IRequestContent.WithQuery</cref>
        /// </inheritdoc>
        public IRequestCreated WithQuery<TQueryable>([NotNull] TQueryable queryable, [NotNull] Expression<Func<TQueryable, object>> queryExpression)
        {
            if(queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            if(queryExpression == null)
            {
                throw new ArgumentNullException(nameof(queryExpression));
            }

            _query = _queryFactory.Build(queryable, queryExpression);

            return this;
        }

        /// <see>
        ///     <cref>IRequestEndpoint.WithEndpoint</cref>
        /// </see>
        public IRequestContent WithEndpoint([NotNull] object parameter)
        {
            if(parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            _endpoint = parameter.ToString();

            return this;
        }

        /// <see>
        ///     <cref>IRequestEndpoint.WithEndpoint</cref>
        /// </see>
        public IRequestContent WithEndpoint([NotNull] string endpoint)
        {
            if(string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            _endpoint = endpoint;

            return this;
        }

        /// <see cref="IRequestEndpoint.WithEndpointTemplate"/>
        public IRequestContent WithEndpointTemplate([NotNull] string template, [NotNull] params object[] parameters)
        {
            if(string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException(nameof(template));
            }

            if(parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if(parameters.Length <= 0)
            {
                throw new ArgumentException(nameof(parameters));
            }

            _endpoint = template;
            _endpointParameters = parameters;

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

            if(!string.IsNullOrWhiteSpace(_endpoint))
            {
                _routeFactory.AddEndpoint(_endpoint);
            }

            if(_endpointParameters != null)
            {
                _routeFactory.AddParameters(_endpointParameters);
            }

            if(!string.IsNullOrWhiteSpace(_query))
            {
                _routeFactory.AddQuery(_query);
            }

            Uri endpoint = _routeFactory.BuildRoute();

            return new ApiRequest(_method, endpoint, _requestContent);
        }
    }
}
