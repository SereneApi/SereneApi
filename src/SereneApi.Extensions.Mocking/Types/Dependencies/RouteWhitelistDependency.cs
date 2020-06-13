using SereneApi.Extensions.Mocking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Types.Dependencies
{
    public class RouteWhitelistDependency : IWhitelist
    {
        public IReadOnlyList<Uri> WhitelistedRoutes { get; }

        public RouteWhitelistDependency(List<Uri> routeWhitelist)
        {
            WhitelistedRoutes = routeWhitelist;
        }

        public bool Validate(object value)
        {
            bool validated = true;

            if (value is Uri route)
            {
                validated = WhitelistedRoutes.Contains(route);
            }

            return validated;
        }
    }
}
