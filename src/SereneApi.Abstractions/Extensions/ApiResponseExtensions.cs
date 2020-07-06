
// Do not change namespace
// ReSharper disable once CheckNamespace

using SereneApi.Abstractions.Responses;

namespace SereneApi.Abstractions
{
    public static class ApiResponseExtensions
    {
        public static bool WasNotSuccessful(this IApiResponse response)
        {
            return !response.WasSuccessful;
        }

        public static bool HasNullResult<TResponse>(this IApiResponse<TResponse> response)
        {
            return response.Result == null;
        }
    }
}
