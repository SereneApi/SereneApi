namespace SereneApi.Abstractions
{
    public interface IApiResponse<out TEntity> : IApiResponse
    {
        TEntity Result { get; }
    }
}
