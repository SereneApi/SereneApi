namespace DeltaWare.SereneApi.Interfaces
{
    public interface IApiHandlerOptionsBuilder
    {
        IApiHandlerOptions BuildOptions(bool disposeClient = true);
    }
}
