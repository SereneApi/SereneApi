using SereneApi.Helpers;
using SereneApi.Request.Attributes;
using System;
using System.Collections.Generic;

namespace SereneApi.Resource.Source
{
    public sealed class AssemblyApiResourceCollection : IApiResourceCollection
    {
        public IEnumerable<Type> GetApiResourceTypes()
            => DiscoveryHelper.GetInterfacesImplementingAttribute<HttpResourceAttribute>();
    }
}
