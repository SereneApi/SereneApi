namespace SereneApi.Interfaces
{
    public interface IApiHandlerFactory
    {
        TApiHandler Build<TApiHandler>() where TApiHandler : ApiHandler;
    }
}
