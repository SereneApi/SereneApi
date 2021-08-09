
using SereneApi.Core.Responses;
using System;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Response
{
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// Specifies if the response was not successful.
        /// </summary>
        public static bool HasNullData<TResponse>([NotNull] this IApiResponse<TResponse> response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return response.Data == null;
        }
    }
}
