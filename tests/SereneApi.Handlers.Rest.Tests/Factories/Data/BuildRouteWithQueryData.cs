using System;
using System.Collections;
using System.Collections.Generic;

namespace SereneApi.Handlers.Rest.Tests.Factories.Data
{
    public abstract class BuildRouteWithQueryData : IEnumerable<object>
    {
        public Uri ExpectedOutput { get; protected set; }

        public Dictionary<string, string> Query { get; protected set; }

        public IEnumerator<object> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}