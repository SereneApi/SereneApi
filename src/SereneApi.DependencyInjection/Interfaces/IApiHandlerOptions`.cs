// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi
{
    public interface IApiHandlerOptions<TApiHandler> : IApiHandlerOptions where TApiHandler : ApiHandler
    {
    }
}
