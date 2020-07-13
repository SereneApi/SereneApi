using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Response;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Options
{
    /// <summary>
    /// Extends <see cref="IApiOptions"/>.
    /// </summary>
    public interface IApiOptionsExtensions
    {
        /// <summary>
        /// Adds an authentication API. Before a request is made it will be authenticated.
        /// The extracted token will be re-used until it expires at which point the authentication API will retrieve a new one.
        /// </summary>
        /// <typeparam name="TApi">The API that will be making the authentication request.</typeparam>
        /// <typeparam name="TDto">The DTO returned by the authentication API.</typeparam>
        /// <param name="callApi">Perform the authentication request.</param>
        /// <param name="extractToken">Extract the token information from the response.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IApiOptionsExtensions AddAuthenticator<TApi, TDto>([NotNull] Func<TApi, Task<IApiResponse<TDto>>> callApi,
            [NotNull] Func<TDto, TokenInfo> extractToken) where TApi : class, IDisposable where TDto : class;
    }
}
