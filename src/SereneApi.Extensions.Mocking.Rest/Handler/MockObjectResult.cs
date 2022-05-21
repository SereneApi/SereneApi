using SereneApi.Core.Http.Responses;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler
{
    internal class MockObjectResult : MockStatusResult
    {
        public object Result { get; }

        public Type Type { get; }

        public MockObjectResult(Status status, object result) : base(status)
        {
            Result = result;
            Type = result.GetType();
        }
    }
}
