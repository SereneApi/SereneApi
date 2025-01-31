using SereneApi.Resource.Source;
using System;
using System.Collections.Generic;

namespace SereneApi.Extensions.DependencyInjection
{
    internal sealed class DependencyInjectionApiResourceProvider : IApiResourceCollection
    {
        public IEnumerable<Type> GetApiResourceTypes()
        {
            throw new NotImplementedException();
        }
    }
}
