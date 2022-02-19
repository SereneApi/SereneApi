using SereneApi.Core.Http;
using SereneApi.Core.Http.Requests.Options;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Requests;
using SereneApi.Core.Serialization;
using SereneApi.Core.Transformation;
using SereneApi.Core.Versioning;
using SereneApi.Handlers.Rest.Routing;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public class RestRequestFactory : IApiRequestFactory
    {
        private readonly RestApiRequest _apiRequest;
        private readonly IRouteFactory _routeFactory;
        private readonly ISerializer _serializer;
        private readonly ITransformationService _transformation;

        public RestApiHandler Handler { get; set; }

        public RestRequestFactory(ITransformationService transformation, ISerializer serializer, IConnectionSettings connection, IRouteFactory routeFactory)
        {
            _transformation = transformation;
            _serializer = serializer;
            _routeFactory = routeFactory;

            _apiRequest = RestApiRequest.Create(connection);
        }

        public IApiRequestResponseType AddInBodyContent<TContent>(TContent content)
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

        public IApiRequestResource UsingMethod(Method method)
        {
            if (method == Method.None)
            {
                throw new ArgumentException("Must use a valid Method.", nameof(method));
            }

            _apiRequest.Method = method;

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

        public IApiRequestType WithQuery<TQuery>(TQuery query) where TQuery : class
        {
            _apiRequest.Query = _transformation.BuildDictionary(query);

            return this;
        }

        public IApiRequestType WithQuery<TQuery>(TQuery query, Expression<Func<TQuery, object>> selector) where TQuery : class
        {
            _apiRequest.Query = _transformation.BuildDictionary(query, selector);

            return this;
        }

        public IApiRequestType WithQuery<TQuery>(Action<TQuery> builder) where TQuery : class
        {
            TQuery query = Activator.CreateInstance<TQuery>();

            builder.Invoke(query);

            _apiRequest.Query = _transformation.BuildDictionary(query);

            return this;
        }

        protected virtual void GenerateRoute()
        {
            _apiRequest.Endpoint = _routeFactory.BuildEndPoint(_apiRequest);
            _apiRequest.Route = _routeFactory.BuildRoute(_apiRequest);
        }
    }
}