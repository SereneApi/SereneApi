using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Http;
using SereneApi.Handlers.Rest.Configuration;
using SereneApi.Handlers.Rest.Requests;
using SereneApi.Handlers.Rest.Routing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SereneApi.Handlers.Rest.Tests.Factories
{
    public class RouteFactoryShould : IDisposable
    {
        private readonly IDependencyProvider _dependencies;

        private readonly IRouteFactory _routeFactory;

        public RouteFactoryShould()
        {
            RestHandlerConfigurationProvider configuration = new RestHandlerConfigurationProvider();

            _dependencies = configuration.Dependencies.BuildProvider();

            _routeFactory = _dependencies.GetRequiredDependency<IRouteFactory>();
        }

        [Theory]
        [InlineData("{0}/Friends", 10, "10/Friends")]
        [InlineData("Friends/{0}", 10, "Friends/10")]
        [InlineData("Friends", 10, "Friends/10")]
        [InlineData("", 10, "10")]
        public void BuildEndpointWithTemplateOneParameter(string endpointTemplate, int parameter, string expected)
        {
            #region Arrange

            Uri expectedRoute = new Uri($"api/Users/{expected}", UriKind.Relative);

            IConnectionSettings connection = new ConnectionSettings("http://noteused/", null, "api");

            RestApiRequest request = RestApiRequest.Create(connection);

            request.Resource = "Users";
            request.EndpointTemplate = endpointTemplate;
            request.Parameters = new object[] { parameter };

            #endregion Arrange

            request.Endpoint = Should.NotThrow(() => _routeFactory.BuildEndPoint(request));

            Uri route = Should.NotThrow(() => _routeFactory.BuildRoute(request));

            #region #region Assert

            route.ShouldBe(expectedRoute);

            #endregion #region Assert
        }

        [Theory]
        [InlineData("{0}/Friends/{1}", 10, "John", "10/Friends/John")]
        [InlineData("{1}/Friends/{0}", 10, "Alfred", "Alfred/Friends/10")]
        public void BuildEndpointWithTemplateTwoParameter(string endpointTemplate, int parameterA, string parameterB, string
        expected)
        {
            #region Arrange

            Uri expectedRoute = new Uri($"api/Users/{expected}", UriKind.Relative);

            IConnectionSettings connection = new ConnectionSettings("http://noteused/", null, "api");

            RestApiRequest request = RestApiRequest.Create(connection);

            request.Resource = "Users";
            request.EndpointTemplate = endpointTemplate;
            request.Parameters = new object[] { parameterA, parameterB };

            #endregion Arrange

            request.Endpoint = Should.NotThrow(() => _routeFactory.BuildEndPoint(request));

            Uri route = Should.NotThrow(() => _routeFactory.BuildRoute(request));

            #region Assert

            route.ShouldBe(expectedRoute);

            #endregion Assert
        }

        [Theory]
        [InlineData("api", "Users", "api/Users")]
        [InlineData("path/api", "Accounts/Details", "path/api/Accounts/Details")]
        public void BuildResourceEndpoint(string resource, string endpointTemplate, string expected)
        {
            #region Arrange

            Uri expectedRoute = new Uri(expected, UriKind.Relative);

            IConnectionSettings connection = new ConnectionSettings("http://noteused/", resource);

            RestApiRequest request = RestApiRequest.Create(connection);

            request.EndpointTemplate = endpointTemplate;

            #endregion Arrange

            request.Endpoint = Should.NotThrow(() => _routeFactory.BuildEndPoint(request));

            //Uri route = Should.NotThrow(() => _routeFactory.BuildRoute(request));
            Uri route = _routeFactory.BuildRoute(request);

            #region Assert

            route.ShouldBe(expectedRoute);

            #endregion Assert
        }

        [Theory]
        [InlineData("api", "Users", "Details", "api/Users/Details")]
        [InlineData("path/api", "Accounts/Details", "Banking", "path/api/Accounts/Details/Banking")]
        public void BuildResourcePathEndpoint(string resourcePath, string resource, string endpointTemplate, string expected)
        {
            #region Arrange

            Uri expectedRoute = new Uri(expected, UriKind.Relative);

            IConnectionSettings connection = new ConnectionSettings("http://noteused/", null, resourcePath);

            RestApiRequest request = RestApiRequest.Create(connection);

            request.EndpointTemplate = endpointTemplate;
            request.Resource = resource;

            #endregion Arrange

            request.Endpoint = Should.NotThrow(() => _routeFactory.BuildEndPoint(request));

            Uri route = Should.NotThrow(() => _routeFactory.BuildRoute(request));

            #region Assert

            route.ShouldBe(expectedRoute);

            #endregion Assert
        }

        [Theory]
        [InlineData("Name-john|Age-18", "api/Users?Name=john&Age=18")]
        [InlineData("Name-john|Country-AU|Age-18", "api/Users?Name=john&Country=AU&Age=18")]
        public void BuildRouteWithQuery(string queryBuild, string expected)
        {
            Dictionary<string, string> query = queryBuild
                .Split('|')
                .ToDictionary(k => k.Split('-')[0], v => v.Split('-')[1]);

            #region Arrange

            Uri expectedRoute = new Uri(expected, UriKind.Relative);

            IConnectionSettings connection = new ConnectionSettings("http://noteused/", null, "api");

            RestApiRequest request = RestApiRequest.Create(connection);

            request.Resource = "Users";
            request.Query = query;

            #endregion Arrange

            request.Endpoint = Should.NotThrow(() => _routeFactory.BuildEndPoint(request));

            Uri route = Should.NotThrow(() => _routeFactory.BuildRoute(request));

            #region Assert

            route.ShouldBe(expectedRoute);

            #endregion Assert
        }

        public void Dispose()
        {
            _dependencies?.Dispose();
        }

        [Fact]
        public void ThrowExceptionWhenMultipleParametersExpectedA()
        {
            #region Arrange

            RestApiRequest request = new RestApiRequest
            {
                EndpointTemplate = "{0}/Details/{1}",
                Parameters = new object[] { 10 }
            };

            #endregion Arrange

            #region Assert

            Should.Throw<FormatException>(() => _routeFactory.BuildEndPoint(request));

            #endregion Assert
        }

        [Fact]
        public void ThrowExceptionWhenMultipleParametersExpectedB()
        {
            #region Arrange

            RestApiRequest request = new RestApiRequest
            {
                EndpointTemplate = "{0}/Details/{1}/{2}",
                Parameters = new object[] { 10, "John" }
            };

            #endregion Arrange

            #region Assert

            Should.Throw<FormatException>(() => _routeFactory.BuildEndPoint(request));

            #endregion Assert
        }

        [Fact]
        public void ThrowExceptionWhenMultipleParametersWithoutTemplate()
        {
            #region Arrange

            RestApiRequest request = new RestApiRequest
            {
                Parameters = new object[] { "string", 10 }
            };

            #endregion Arrange

            #region Assert

            Should.Throw<ArgumentException>(() => _routeFactory.BuildEndPoint(request));

            #endregion Assert
        }
    }
}