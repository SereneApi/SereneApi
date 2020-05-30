namespace SereneApi.Interfaces
{
    internal interface IApiHandlerFactory
    {
        TApiHandler Build<TApiHandler>() where TApiHandler : ApiHandler;
    }
}
