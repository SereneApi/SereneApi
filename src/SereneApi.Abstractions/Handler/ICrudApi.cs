using SereneApi.Abstractions.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Handler
{
    /// <summary>
    /// When Inherited; provides the necessary methods for implementing a CRUD API consumer.
    /// </summary>
    /// <typeparam name="TResource">The resource type the API will be acting on, both send and receive.</typeparam>
    /// <typeparam name="TIdentifier">The identifier type used by the resource, this could be a <see cref="Guid"/>, <see cref="long"/> or <see cref="int"/>.</typeparam>
    public interface ICrudApi<TResource, in TIdentifier> where TResource : class where TIdentifier : struct
    {
        /// <summary>
        /// Performs a GET request against the API.
        /// </summary>
        /// <param name="identifier">The resource identity.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<IApiResponse<TResource>> GetAsync([NotNull] TIdentifier identifier);

        /// <summary>
        /// Performs a GET request against the API returning all resources.
        /// </summary>
        Task<IApiResponse<List<TResource>>> GetAllAsync();

        /// <summary>
        /// Performs a POST request against the API.
        /// </summary>
        /// <param name="resource">The resource to be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<IApiResponse<TResource>> CreateAsync([NotNull] TResource resource);

        /// <summary>
        /// Performs a DELETE request against the API.
        /// </summary>
        /// <param name="identifier">The resource to be deleted.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<IApiResponse> DeleteAsync([NotNull] TIdentifier identifier);

        /// <summary>
        /// Performs a PUT request against the API.
        /// </summary>
        /// <param name="resource">The resource to be replaced.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<IApiResponse<TResource>> ReplaceAsync([NotNull] TResource resource);

        /// <summary>
        /// Performs a PATCH request against the API.
        /// </summary>
        /// <param name="resource">The resource to be updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<IApiResponse<TResource>> UpdateAsync([NotNull] TResource resource);
    }
}
