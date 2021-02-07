using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Response;
using System;

namespace SereneApi.Abstractions.Request
{
    public class RequestOptions: IRequestOptions
    {
        public IDependencyProvider Dependencies { get; }

        public RequestOptions(IDependencyProvider dependencies)
        {
            Dependencies = dependencies;
        }

        public void OnFailedRequest(Action onFailure)
        {
            throw new NotImplementedException();
        }

        public void OnStatus(Action<Status> onStatus)
        {
            throw new NotImplementedException();
        }

        public void OnException(Action<Exception> onException)
        {
            throw new NotImplementedException();
        }
    }
}
