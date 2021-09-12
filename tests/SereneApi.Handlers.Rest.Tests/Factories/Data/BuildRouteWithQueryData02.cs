using System;

namespace SereneApi.Handlers.Rest.Tests.Factories.Data
{
    internal class BuildRouteWithQueryData02 : BuildRouteWithQueryData
    {
        public BuildRouteWithQueryData02()
        {
            ExpectedOutput = new Uri("api/Users?Name=john&Country=AU&Age=18", UriKind.Relative);
            Query = new()
            {
                { "Name", "John" },
                { "Age", "18" },
                { "Country", "AU" }
            };
        }
    }
}