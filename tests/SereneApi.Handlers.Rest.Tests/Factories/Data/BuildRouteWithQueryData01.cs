using System;

namespace SereneApi.Handlers.Rest.Tests.Factories.Data
{
    internal class BuildRouteWithQueryData01 : BuildRouteWithQueryData
    {
        public BuildRouteWithQueryData01()
        {
            ExpectedOutput = new Uri("api/Users?Name=john&Age=18", UriKind.Relative);
            Query = new()
            {
                { "Name", "John" },
                { "Age", "18" }
            };
        }
    }
}