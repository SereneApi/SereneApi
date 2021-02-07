using SereneApi.Abstractions.Response;
using System;

namespace SereneApi.Abstractions.Request
{
    public interface IRequestOptions
    {
        void OnFailedRequest(Action onFailure);

        void OnStatus(Action<Status> onStatus);

        void OnException(Action<Exception> onException);
    }
}
