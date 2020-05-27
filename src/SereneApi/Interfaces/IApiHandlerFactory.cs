namespace DeltaWare.SereneApi.Interfaces
{
    internal interface IApiHandlerFactory
    {
        TApiHandler CreateInstance<TApiHandler>() where TApiHandler : ApiHandler;
    }
}
