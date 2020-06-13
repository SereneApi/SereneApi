using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Extensions.Mocking.Types.Dependencies;
using SereneApi.Interfaces;
using SereneApi.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Types
{
    public class MockResponse : CoreOptions, IMockResponse, IWhitelist
    {
        private readonly IApiRequestContent _response;

        public ISerializer Serializer { get; }

        public MockResponse(IApiRequestContent response, ISerializer serializer)
        {
            _response = response;

            Serializer = serializer;

            DependencyCollection.AddDependency(serializer);
        }

        public bool Validate(object value)
        {
            List<IWhitelist> whitelistDependencies = DependencyCollection.GetDependencies<IWhitelist>();

            // If 0 or any whitelist items return true. True is returned.
            return whitelistDependencies.Count == 0 || whitelistDependencies.Any(w => w.Validate(value));
        }

        public async Task<IApiRequestContent> GetResponseAsync()
        {
            if (DependencyCollection.TryGetDependency(out DelayResponseDependency delay))
            {
                await delay.DelayAsync();
            }

            return _response;
        }

        public IMockResponseExtensions GetExtensions()
        {
            return new MockResponseExtensions(DependencyCollection);
        }
    }
}
