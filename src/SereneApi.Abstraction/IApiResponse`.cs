namespace SereneApi.Abstraction
{
    public interface IApiResponse<out TEntity> : IApiResponse
    {
        TEntity Result { get; }
    }
}
