namespace DeltaWare.SereneApi.Interfaces
{
    public interface IApiResponse<out TEntity> : IApiResponse
    {
        TEntity Result { get; }
    }
}
