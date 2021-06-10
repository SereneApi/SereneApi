using SereneApi.Abstractions.Requests;

namespace SereneApi.Requests
{
    public interface IApiRequestBuilder : IApiRequestResource
    {
        IApiRequestResource UsingMethod(Method method);
    }
}
