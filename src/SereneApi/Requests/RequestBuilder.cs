using SereneApi.Abstractions;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response;
using SereneApi.Abstractions.Routing;
using SereneApi.Abstractions.Serialization;
using SereneApi.Extensions;
using SereneApi.Requests.Types;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SereneApi.Requests
{
    internal class RequestBuilder : IApiRequestBuilder, IApiRequestResource
    {
        private readonly BaseApiHandler _apiHandler;

        private readonly ApiRequest _apiRequest;

        public RequestBuilder(BaseApiHandler apiHandler)
        {
            _apiHandler = apiHandler;
            _apiRequest = _apiHandler.Connection.GenerateApiRequest();
        }

        public IApiRequestResource UsingMethod(Method method)
        {
            if (method == Method.NONE)
            {
                throw new ArgumentException("Must use a valid Method.", nameof(method));
            }

            _apiRequest.Method = method;

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
            _apiRequest.Version = new ApiVersion(version);

            return this;
        }

        public IApiRequestEndpoint AgainstVersion(IApiVersion version)
        {
            _apiRequest.Version = version ?? throw new ArgumentNullException(nameof(version));

            return this;
        }

        public IApiRequestQuery WithParameter(object parameter)
        {
            _apiRequest.Parameters = new[] { parameter ?? throw new ArgumentNullException(nameof(parameter)) };

            return this;
        }

        public IApiRequestParameters WithEndpoint(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            _apiRequest.EndpointTemplate = endpoint;

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

            if (string.IsNullOrWhiteSpace(_apiRequest.Endpoint))
            {
                throw new MethodAccessException("An EndPoint must be specified before parameters can be added.");
            }

            _apiRequest.Parameters = parameters;

            return this;
        }

        public IApiRequestContent WithQuery<TQuery>(TQuery query)
        {
            IQueryFactory queryFactory = _apiHandler.Options.Dependencies.GetDependency<IQueryFactory>();

            _apiRequest.Query = queryFactory.BuildDictionary(query);

            return this;
        }

        public IApiRequestContent WithQuery<TQuery>(TQuery query, Expression<Func<TQuery, object>> selector)
        {
            IQueryFactory queryFactory = _apiHandler.Options.Dependencies.GetDependency<IQueryFactory>();

            _apiRequest.Query = queryFactory.BuildDictionary(query, selector);

            return this;
        }

        public IApiRequestResponseContent AddInBodyContent<TContent>(TContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            ISerializer serializer = _apiHandler.Options.Dependencies.GetDependency<ISerializer>();

            _apiRequest.Content = serializer.Serialize(content);

            return this;
        }

        public IApiRequestPerformer<TContent> RespondsWithContent<TContent>()
        {
            return new RequestBuilder<TContent>(_apiHandler, _apiRequest);
        }

        public IApiResponse Execute()
        {
            GenerateRoute();

            return _apiHandler.PerformRequest(_apiRequest);
        }

        public Task<IApiResponse> ExecuteAsync()
        {
            GenerateRoute();

            return _apiHandler.PerformRequestAsync(_apiRequest);
        }

        private void GenerateRoute()
        {
            IRouteFactory routeFactory = _apiHandler.Options.Dependencies.GetDependency<IRouteFactory>();

            _apiRequest.Endpoint = routeFactory.BuildEndPoint(_apiRequest);
            _apiRequest.Route = routeFactory.BuildRoute(_apiRequest);
        }
    }
}
