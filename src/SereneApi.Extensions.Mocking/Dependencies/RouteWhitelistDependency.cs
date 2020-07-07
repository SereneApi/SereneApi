using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Dependencies
{
    public class RouteWhitelistDependency: IWhitelist
    {
        public IReadOnlyList<Uri> WhitelistedRoutes { get; }

        public RouteWhitelistDependency(List<Uri> routeWhitelist)
        {
            WhitelistedRoutes = routeWhitelist;
        }

        public Validity Validate(object value)
        {
            if(!(value is Uri route))
            {
                return Validity.NotApplicable;
            }

            if(WhitelistedRoutes.Contains(route))
            {
                return Validity.Valid;
            }

            return Validity.Invalid;
        }
    }
}
