using SereneApi.Core.Responses;
using System;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Response
{
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// Specifies if the response was not successful.
        /// </summary>
        public static bool HasNullData<TResponse>(this IApiResponse<TResponse> response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return response.Data == null;
        }
    }
}