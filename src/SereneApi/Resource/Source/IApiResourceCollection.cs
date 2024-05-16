using System;
using System.Collections.Generic;

namespace SereneApi.Resource.Source
{
    public interface IApiResourceCollection
    {
        IEnumerable<Type> GetApiResourceTypes();
    }
}
