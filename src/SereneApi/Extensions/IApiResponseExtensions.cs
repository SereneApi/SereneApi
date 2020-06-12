// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi
{
    public static class IApiResponseExtensions
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
