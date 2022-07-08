using DeltaWare.SDK.Serialization.Types;
using SereneApi.Core.Handler;
using SereneApi.Core.Http;
using SereneApi.Core.Http.Requests.Options;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Serialization;
using SereneApi.Core.Versioning;
using SereneApi.Handlers.Rest.Routing;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public class RestRequestFactory : IApiRequestFactory
    {
        private readonly RestApiRequest _apiRequest;
        private readonly IRouteFactory _routeFactory;
        private readonly ISerializer _serializer;
        private readonly IObjectSerializer _objectSerializer;

        public IApiHandler Handler { get; set; }

        public RestRequestFactory(IObjectSerializer objectSerializer, ISerializer serializer, IConnectionSettings connection, IRouteFactory routeFactory)
        {
            _objectSerializer = objectSerializer;
            _serializer = serializer;
            _routeFactory = routeFactory;

            _apiRequest = RestApiRequest.Create(connection);
        }

        [Obsolete]
        public IApiRequestResponseType AddInBodyContent<TContent>(TContent content) => WithInBodyContent(content);

        public IApiRequestResponseType WithInBodyContent<TContent>(TContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            _apiRequest.Content = _serializer.Serialize(content);
            _apiRequest.ContentType = typeof(TContent);

            return this;
        }

        public IApiRequestParameters AgainstEndpoint(string endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException(nameof(endpoint));
            }

            _apiRequest.EndpointTemplate = endpoint;

            return this;
        }

        public IApiRequestVersion AgainstResource(string resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException(nameof(resource));
            }

            _apiRequest.Resource = resource;

            return this;
        }

        public IApiRequestEndpoint AgainstVersion(string version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException(nameof(version));
            }

            _apiRequest.Version = new ApiVersion(version);

            return this;
        }

        public IApiRequestEndpoint AgainstVersion(IApiVersion version)
        {
            _apiRequest.Version = version ?? throw new ArgumentNullException(nameof(version));

            return this;
        }

        public Task<IApiResponse> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            GenerateRoute();

            return Handler.PerformRequestAsync(_apiRequest, ApiRequestOptions.Default, cancellationToken);
        }

        public Task<IApiResponse> ExecuteAsync(Action<IApiRequestOptionsBuilder> optionsBuilder, CancellationToken cancellationToken = default)
        {
            GenerateRoute();

            ApiRequestOptions options = ApiRequestOptions.Default;

            optionsBuilder.Invoke(options);

            return Handler.PerformRequestAsync(_apiRequest, options, cancellationToken);
        }

        public IApiRequestPerformer<TContent> RespondsWith<TContent>()
        {
            _apiRequest.ResponseType = typeof(TContent);

            return new RestRequestFactory<TContent>(Handler, _apiRequest);
        }

        public IApiRequestResource UsingMethod(HttpMethod httpMethod)
        {
            _apiRequest.HttpMethod = httpMethod;

            return this;
        }

        public IApiRequestQuery WithParameter(object parameter)
        {
            _apiRequest.Parameters = new[] { parameter ?? throw new ArgumentNullException(nameof(parameter)) };

            return this;
        }

        public IApiRequestQuery WithParameters(params object[] parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.Length <= 0)
            {
                throw new ArgumentException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(_apiRequest.EndpointTemplate))
            {
                throw new MethodAccessException("An EndPoint must be specified before parameters can be added.");
            }

            _apiRequest.Parameters = parameters;

            return this;
        }

        public IApiRequestHeaders WithQuery<TQuery>(TQuery query) where TQuery : class
        {
            _apiRequest.Query = _objectSerializer.SerializeToDictionary(query);

            return this;
        }

        public IApiRequestHeaders WithQuery<TQuery>(TQuery query, Expression<Func<TQuery, object>> selector) where TQuery : class
        {
            _apiRequest.Query = _objectSerializer.SerializeToDictionary(query, selector);

            return this;
        }

        public IApiRequestHeaders WithQuery<TQuery>(Action<TQuery> builder) where TQuery : class
        {
            TQuery query = Activator.CreateInstance<TQuery>();

            builder.Invoke(query);

            _apiRequest.Query = _objectSerializer.SerializeToDictionary(query);

            return this;
        }

        protected virtual void GenerateRoute()
        {
            _apiRequest.Endpoint = _routeFactory.BuildEndPoint(_apiRequest);
            _apiRequest.Route = _routeFactory.BuildRoute(_apiRequest);
        }

        public IApiRequestBody WithHeaders(Action<IHeaderBuilder> headerBuilder)
        {
            throw new NotImplementedException();
        }

        public IApiRequestBody WithHeaders(Dictionary<string, object> headers)
        {
            throw new NotImplementedException();
        }

        public IApiRequestBody WithHeaders<THeader>(THeader header)
        {
            throw new NotImplementedException();
        }
    }
}