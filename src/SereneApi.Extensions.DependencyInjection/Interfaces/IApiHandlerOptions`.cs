namespace SereneApi.Extensions.DependencyInjection.Interfaces
{
    public interface IApiHandlerOptions<TApiHandler>: IApiHandlerOptions where TApiHandler : ApiHandler
    {
    }
}
