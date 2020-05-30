namespace SereneApi.DependencyInjection
{
    public interface IApiHandlerOptions<TApiHandler> : IApiHandlerOptions where TApiHandler : ApiHandler
    {
    }
}
