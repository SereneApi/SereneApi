using SereneApi.Factories;
using SereneApi.Interfaces;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Tests.Factories
{
    public class RouteFactoryShould
    {
        [Fact]
        public void ThrowExceptionWhenMultipleParametersWithoutTemplate()
        {
            #region Arrange

            IRouteFactory routeFactory = new RouteFactory();

            routeFactory.AddParameters("string", 10);

            #endregion

            #region Assert

            Should.Throw<ArgumentException>(() => routeFactory.BuildRoute());

            #endregion
        }

        [Fact]
        public void ThrowExceptionWhenMultipleParametersExpectedA()
        {
            #region Arrange

            IRouteFactory routeFactory = new RouteFactory();

            routeFactory.AddEndPoint("{0}/Details/{1}");
            routeFactory.AddParameters(10);

            #endregion

            #region Assert

            Should.Throw<FormatException>(() => routeFactory.BuildRoute());

            #endregion
        }

        [Fact]
        public void ThrowExceptionWhenMultipleParametersExpectedB()
        {
            #region Arrange

            IRouteFactory routeFactory = new RouteFactory();

            routeFactory.AddEndPoint("{0}/Details/{1}/{2}");
            routeFactory.AddParameters(10, "John");

            #endregion

            #region Assert

            Should.Throw<FormatException>(() => routeFactory.BuildRoute());

            #endregion
        }


        [Theory]
        [InlineData("api", "Users", "api/Users")]
        [InlineData("api/", "Users/", "api/Users")]
        [InlineData("path/api", "Accounts/Details", "path/api/Accounts/Details")]
        [InlineData("path/api/", "Accounts/Details/", "path/api/Accounts/Details")]
        public void BuildResourceEndpoint(string resource, string endPoint, string expected)
        {
            #region Arrange

            Uri expectedRoute = new Uri(expected, UriKind.Relative);

            IRouteFactory routeFactory = new RouteFactory();

            routeFactory.AddResource(resource);
            routeFactory.AddEndPoint(endPoint);

            #endregion
            #region Act

            Uri route = Should.NotThrow(() => routeFactory.BuildRoute());

            #endregion
            #region Assert

            route.ShouldBe(expectedRoute);

            #endregion
        }

        [Theory]
        [InlineData("api", "Users", "Details", "api/Users/Details")]
        [InlineData("api/", "Users/", "Details/", "api/Users/Details")]
        [InlineData("path/api", "Accounts/Details", "Banking", "path/api/Accounts/Details/Banking")]
        [InlineData("path/api/", "Accounts/Details/", "Banking/", "path/api/Accounts/Details/Banking")]
        public void BuildResourcePathEndpoint(string resourcePath, string resource, string endPoint, string expected)
        {
            #region Arrange

            Uri expectedRoute = new Uri(expected, UriKind.Relative);

            IRouteFactory routeFactory = new RouteFactory(resourcePath);

            routeFactory.AddResource(resource);
            routeFactory.AddEndPoint(endPoint);

            #endregion
            #region Act

            Uri route = Should.NotThrow(() => routeFactory.BuildRoute());

            #endregion
            #region Assert

            route.ShouldBe(expectedRoute);

            #endregion
        }

        [Theory]
        [InlineData("{0}/Friends", 10, "10/Friends")]
        [InlineData("Friends/{0}", 10, "Friends/10")]
        [InlineData("Friends", 10, "Friends/10")]
        [InlineData("", 10, "10")]
        public void BuildEndpointWithTemplateOneParameter(string endPoint, int parameter, string expected)
        {
            #region Arrange

            Uri expectedRoute = new Uri($"api/Users/{expected}", UriKind.Relative);

            IRouteFactory routeFactory = new RouteFactory("api/");

            routeFactory.AddResource("Users");
            routeFactory.AddEndPoint(endPoint);
            routeFactory.AddParameters(parameter);

            #endregion
            #region Act

            Uri route = Should.NotThrow(() => routeFactory.BuildRoute());

            #endregion
            #region Assert

            route.ShouldBe(expectedRoute);

            #endregion
        }

        [Theory]
        [InlineData("{0}/Friends/{1}", 10, "John", "10/Friends/John")]
        [InlineData("{1}/Friends/{0}", 10, "Alfred", "Alfred/Friends/10")]
        public void BuildEndpointWithTemplateTwoParameter(string endPoint, int parameterA, string parameterB, string expected)
        {
            #region Arrange

            Uri expectedRoute = new Uri($"api/Users/{expected}", UriKind.Relative);

            IRouteFactory routeFactory = new RouteFactory("api/");

            routeFactory.AddResource("Users");
            routeFactory.AddEndPoint(endPoint);
            routeFactory.AddParameters(parameterA, parameterB);

            #endregion
            #region Act

            Uri route = Should.NotThrow(() => routeFactory.BuildRoute());

            #endregion
            #region Assert

            route.ShouldBe(expectedRoute);

            #endregion
        }

        [Theory]
        [InlineData("?Name=john&Age=18", "api/Users?Name=john&Age=18")]
        [InlineData("?Name=john&Country=AU&Age=18", "api/Users?Name=john&Country=AU&Age=18")]
        public void BuildRouteWithQuery(string query, string expected)
        {
            #region Arrange

            Uri expectedRoute = new Uri(expected, UriKind.Relative);

            IRouteFactory routeFactory = new RouteFactory("api/");

            routeFactory.AddResource("Users");
            routeFactory.AddQuery(query);

            #endregion
            #region Act

            Uri route = Should.NotThrow(() => routeFactory.BuildRoute());

            #endregion
            #region Assert

            route.ShouldBe(expectedRoute);

            #endregion
        }
    }
}
