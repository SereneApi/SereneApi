using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Response;
using System;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Options
{
    public interface IApiOptionsExtensions
    {
        IApiOptionsExtensions AddAuthenticator<TApi, TDto>(Func<TApi, Task<IApiResponse<TDto>>> callApiFunction,
            Func<TDto, TokenInfo> getTokenInfo) where TApi : class, IDisposable where TDto : class;
    }
}
