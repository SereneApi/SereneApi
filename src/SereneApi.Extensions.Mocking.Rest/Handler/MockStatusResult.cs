using SereneApi.Core.Http.Responses;

namespace SereneApi.Extensions.Mocking.Rest.Handler
{
    internal class MockStatusResult : IMockResult
    {
        public Status Status { get; }

        public MockStatusResult(Status status)
        {
            Status = status;
        }
    }
}
