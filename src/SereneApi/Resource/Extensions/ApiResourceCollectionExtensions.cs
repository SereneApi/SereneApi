using SereneApi.Resource.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace SereneApi.Resource.Source
{
    internal static class ApiResourceCollectionExtensions
    {
        public static IReadOnlyDictionary<Type, ApiResourceSchema> GetApiResourcesAsDictionary(this IApiResourceCollection resourceCollection)
            => resourceCollection
                .GetApiResourceTypes()
                .ToDictionary(apiResource => apiResource, ApiResourceSchema.Create);
    }
}
