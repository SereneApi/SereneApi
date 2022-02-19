using SereneApi.Core.Http.Responses.Exceptions;
using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Http.Responses
{
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// Awaits the <see cref="IApiResponse"/> and returns the data.
        /// </summary>
        /// <returns>
        /// Returns the <see cref="IApiResponse"/> data, returns null if no data or a failed
        /// response was received.
        /// </returns>
        public static async Task<TResponse> GetDataAsync<TResponse>(this Task<IApiResponse<TResponse>> response)
        {
            return (await response).Data;
        }

        /// <summary>
        /// Awaits the <see cref="IApiResponse"/> and returns the <see cref="Status"/>.
        /// </summary>
        public static async Task<Status> GetStatusAsync<TResponse>(this Task<IApiResponse<TResponse>> response)
        {
            return (await response).Status;
        }

        /// <summary>
        /// Awaits the <see cref="IApiResponse"/> and returns the <see cref="Status"/>.
        /// </summary>
        public static async Task<Status> GetStatusAsync(this Task<IApiResponse> response)
        {
            return (await response).Status;
        }

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

        /// <summary>
        /// Throws a <see cref="UnsuccessfulResponseException"/> if the <see cref="IApiResponse"/>
        /// was not successful.
        /// </summary>
        /// <returns>Returns the <see cref="IApiResponse"/>.</returns>
        public static IApiResponse ThrowOnFail(this IApiResponse response)
        {
            if (response.WasSuccessful)
            {
                return response;
            }

            if (response.HasException)
            {
                throw response.Exception;
            }

            throw new UnsuccessfulResponseException(response);
        }

        /// <summary>
        /// Throws a <see cref="UnsuccessfulResponseException"/> if the <see cref="IApiResponse"/>
        /// was not successful.
        /// </summary>
        /// <returns>Returns the <see cref="IApiResponse"/>.</returns>
        public static IApiResponse<TResponse> ThrowOnFail<TResponse>(this IApiResponse<TResponse> response)
        {
            if (response.WasSuccessful)
            {
                return response;
            }

            if (response.HasException)
            {
                throw response.Exception;
            }

            throw new UnsuccessfulResponseException(response);
        }

        /// <summary>
        /// Throws a <see cref="UnsuccessfulResponseException"/> if the <see cref="IApiResponse"/>
        /// was not successful.
        /// </summary>
        /// <returns>Returns the <see cref="IApiResponse"/>.</returns>
        public static async Task<IApiResponse> ThrowOnFail(this Task<IApiResponse> response)
        {
            IApiResponse apiResponse = await response;

            return apiResponse.ThrowOnFail();
        }

        /// <summary>
        /// Throws a <see cref="UnsuccessfulResponseException"/> if the <see cref="IApiResponse"/>
        /// was not successful.
        /// </summary>
        /// <returns>Returns the <see cref="IApiResponse"/>.</returns>
        public static async Task<IApiResponse<TResponse>> ThrowOnFail<TResponse>(this Task<IApiResponse<TResponse>> response)
        {
            IApiResponse<TResponse> apiResponse = await response;

            return apiResponse.ThrowOnFail();
        }
    }
}