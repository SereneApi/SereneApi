using System.Net.Http;

namespace SereneApi.Core.Http.Client.Handler
{
    public interface IHandlerBuilder
    {
        HttpMessageHandler BuildHandler();
    }
}