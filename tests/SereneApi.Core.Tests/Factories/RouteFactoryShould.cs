﻿//using SereneApi.Abstractions;
//using SereneApi.Abstractions.Factories;
//using SereneApi.Abstractions.Types;
//using Shouldly;
//using System;
//using SereneApi.Abstractions.Configuration;
//using Xunit;

//namespace SereneApi.Tests.Factories
//{
//    public class RouteFactoryShould
//    {
//        [Fact]
//        public void ThrowExceptionWhenMultipleParametersWithoutTemplate()
//        {
//            #region Arrange

//            IRouteFactory routeFactory = new DefaultRouteFactory();

//            routeFactory.AddParameters("string", 10);

//            #endregion

//            #region Assert

//            Should.Throw<ArgumentException>(() => routeFactory.BuildRoute());

//            #endregion
//        }

//        [Fact]
//        public void ThrowExceptionWhenMultipleParametersExpectedA()
//        {
//            #region Arrange

//            IRouteFactory routeFactory = new DefaultRouteFactory();

//            routeFactory.AddEndpoint("{0}/Details/{1}");
//            routeFactory.AddParameters(10);

//            #endregion

//            #region Assert

//            Should.Throw<FormatException>(() => routeFactory.BuildRoute());

//            #endregion
//        }

//        [Fact]
//        public void ThrowExceptionWhenMultipleParametersExpectedB()
//        {
//            #region Arrange

//            IRouteFactory routeFactory = new DefaultRouteFactory();

//            routeFactory.AddEndpoint("{0}/Details/{1}/{2}");
//            routeFactory.AddParameters(10, "John");

//            #endregion

//            #region Assert

//            Should.Throw<FormatException>(() => routeFactory.BuildRoute());

//            #endregion
//        }


//        [Theory]
//        [InlineData("api", "Users", "api/Users")]
//        [InlineData("api/", "Users/", "api/Users")]
//        [InlineData("path/api", "Accounts/Details", "path/api/Accounts/Details")]
//        [InlineData("path/api/", "Accounts/Details/", "path/api/Accounts/Details")]
//        public void BuildResourceEndpoint(string resource, string endpoint, string expected)
//        {
//            #region Arrange

//            Uri expectedRoute = new Uri(expected, UriKind.Relative);

//            IRouteFactory routeFactory = new DefaultRouteFactory();

//            routeFactory.AddResource(resource);
//            routeFactory.AddEndpoint(endpoint);

//            #endregion
//            #region Act

//            Uri route = Should.NotThrow(() => routeFactory.BuildRoute());

//            #endregion
//            #region Assert

//            route.ShouldBe(expectedRoute);

//            #endregion
//        }

//        [Theory]
//        [InlineData("api", "Users", "Details", "api/Users/Details")]
//        [InlineData("api/", "Users/", "Details/", "api/Users/Details")]
//        [InlineData("path/api", "Accounts/Details", "Banking", "path/api/Accounts/Details/Banking")]
//        [InlineData("path/api/", "Accounts/Details/", "Banking/", "path/api/Accounts/Details/Banking")]
//        public void BuildResourcePathEndpoint(string resourcePath, string resource, string endpoint, string expected)
//        {
//            #region Arrange

//            Uri expectedRoute = new Uri(expected, UriKind.Relative);

//            IConnectionSettings connection = new Connection("http://noteused/", null, resourcePath);

//            IRouteFactory routeFactory = new DefaultRouteFactory(connection);

//            routeFactory.AddResource(resource);
//            routeFactory.AddEndpoint(endpoint);

//            #endregion
//            #region Act

//            Uri route = Should.NotThrow(() => routeFactory.BuildRoute());

//            #endregion
//            #region Assert

//            route.ShouldBe(expectedRoute);

//            #endregion
//        }

//        [Theory]
//        [InlineData("{0}/Friends", 10, "10/Friends")]
//        [InlineData("Friends/{0}", 10, "Friends/10")]
//        [InlineData("Friends", 10, "Friends/10")]
//        [InlineData("", 10, "10")]
//        public void BuildEndpointWithTemplateOneParameter(string endpoint, int parameter, string expected)
//        {
//            #region Arrange

//            Uri expectedRoute = new Uri($"api/Users/{expected}", UriKind.Relative);

//            IConnectionSettings connection = new Connection("http://noteused/", null, "api/");

//            IRouteFactory routeFactory = new DefaultRouteFactory(connection);

//            routeFactory.AddResource("Users");
//            routeFactory.AddEndpoint(endpoint);
//            routeFactory.AddParameters(parameter);

//            #endregion
//            #region Act

//            Uri route = Should.NotThrow(() => routeFactory.BuildRoute());

//            #endregion
//            #region Assert

//            route.ShouldBe(expectedRoute);

//            #endregion
//        }

//        [Theory]
//        [InlineData("{0}/Friends/{1}", 10, "John", "10/Friends/John")]
//        [InlineData("{1}/Friends/{0}", 10, "Alfred", "Alfred/Friends/10")]
//        public void BuildEndpointWithTemplateTwoParameter(string endpoint, int parameterA, string parameterB, string expected)
//        {
//            #region Arrange

//            Uri expectedRoute = new Uri($"api/Users/{expected}", UriKind.Relative);

//            IConnectionSettings connection = new Connection("http://noteused/", null, "api/");

//            IRouteFactory routeFactory = new DefaultRouteFactory(connection);

//            routeFactory.AddResource("Users");
//            routeFactory.AddEndpoint(endpoint);
//            routeFactory.AddParameters(parameterA, parameterB);

//            #endregion
//            #region Act

//            Uri route = Should.NotThrow(() => routeFactory.BuildRoute());

//            #endregion
//            #region Assert

//            route.ShouldBe(expectedRoute);

//            #endregion
//        }

//        [Theory]
//        [InlineData("?Name=john&Age=18", "api/Users?Name=john&Age=18")]
//        [InlineData("?Name=john&Country=AU&Age=18", "api/Users?Name=john&Country=AU&Age=18")]
//        public void BuildRouteWithQuery(string query, string expected)
//        {
//            #region Arrange

//            Uri expectedRoute = new Uri(expected, UriKind.Relative);

//            IConnectionSettings connection = new Connection("http://noteused/", null, "api/");

//            IRouteFactory routeFactory = new DefaultRouteFactory(connection);

//            routeFactory.AddResource("Users");
//            routeFactory.AddQuery(query);

//            #endregion
//            #region Act

//            Uri route = Should.NotThrow(() => routeFactory.BuildRoute());

//            #endregion
//            #region Assert

//            route.ShouldBe(expectedRoute);

//            #endregion
//        }
//    }
//}
