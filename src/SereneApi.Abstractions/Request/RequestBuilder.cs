using SereneApi.Abstractions.Content;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Routing;
using SereneApi.Abstractions.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace SereneApi.Abstractions.Request
{
    /// <summary>
    ///  Builds an <see cref="IApiRequest"/> based on the supplied values.
    /// </summary>
    public class RequestBuilder: IRequest, IRequestEndPointParams
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

        /// <summary>
        /// Creates a new instance of <see cref="RequestBuilder"/>.
        /// </summary>
        /// <param name="options">The options to be used for building requests.</param>
        /// <param name="resource">The resource that the request is intended for.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public RequestBuilder([NotNull] IApiOptions options, [AllowNull] string resource = null)
        {
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _routeFactory = options.Dependencies.GetDependency<IRouteFactory>();
            _queryFactory = options.Dependencies.GetDependency<IQueryFactory>();
            _serializer = options.Dependencies.GetDependency<ISerializer>();
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
        public IRequestCreated AddInBodyContent<TContent>([NotNull] TContent content)
        {
            if(content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            _requestContent = _serializer.Serialize(content);

            return this;
        }

        public IRequestCreated AddInBodyContent([NotNull] IApiRequestContent content)
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
        public IRequestContent WithParameter([NotNull] object parameter)
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
        public IRequestEndPointParams WithEndPoint([NotNull] string endpoint)
        {
            if(string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            _endpoint = endpoint;

            return this;
        }

        /// <inheritdoc cref="IRequestEndPointParams.WithParameters(object[])"/>
        public IRequestContent WithParameters(params object[] parameters)
        {
            if(parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if(parameters.Length <= 0)
            {
                throw new ArgumentException(nameof(parameters));
            }

            if(string.IsNullOrWhiteSpace(_endpoint))
            {
                throw new MethodAccessException("An EndPoint must be specified before parameters can be added.");
            }

            _endpointParameters = parameters;

            return this;
        }

        /// <inheritdoc cref="IRequestEndPointParams.WithParameters(IList{object})"/>
        public IRequestContent WithParameters(IList<object> parameters)
        {
            return WithParameters(parameters.ToArray());
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
                if(_endpointParameters?.Length > 0)
                {
                    _routeFactory.AddEndpoint(_endpoint);
                    _routeFactory.AddParameters(_endpointParameters);
                }
                else
                {
                    _routeFactory.AddParameters(_endpoint);
                }
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
