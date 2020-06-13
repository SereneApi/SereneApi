using SereneApi.Interfaces;

namespace SereneApi.Types
{
    public class ApiRequest : IApiRequest
    {
        public Uri EndPoint { get; }

        public Method Method { get; }

        public IApiRequestContent Content { get; }

        public ApiRequest(Method method, Uri endPoint = null, IApiRequestContent content = null)
        {
            Method = method;
            EndPoint = endPoint;
            Content = content;
        }
    }

}
