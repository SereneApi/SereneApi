// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi
{
    public interface IApiResponse<out TEntity>: IApiResponse
    {
        TEntity Result { get; }
    }
}
