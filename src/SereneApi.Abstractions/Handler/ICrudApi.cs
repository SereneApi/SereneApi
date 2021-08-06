using SereneApi.Abstractions.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Handler
{
    /// <summary>
    /// When Inherited; provides the necessary methods for implementing a CRUD API consumer.
    /// </summary>
    /// <typeparam name="TResource">Specifies the type that defines the APIs resource, this resource will be retrieved and provided by the API.</typeparam>
    /// <typeparam name="TIdentifier">Specifies the identifier type used by the API to identify the resource, this could be a <see cref="Guid"/>, <see cref="long"/> or <see cref="int"/>.</typeparam>
    public interface ICrudApi<TResource, in TIdentifier> where TResource : class where TIdentifier : struct
    {
        /// <summary>
        /// Performs a GET request against the API.
        /// </summary>
        /// <param name="identifier">The resource identity.</param>
        Task<IApiResponse<TResource>> GetAsync(TIdentifier identifier);

        /// <summary>
        /// Performs a GET request against the API returning all resources.
        /// </summary>
        Task<IApiResponse<List<TResource>>> GetAsync();

        /// <summary>
        /// Performs a POST request against the API.
        /// </summary>
        /// <param name="resource">The resource to be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<IApiResponse<TResource>> CreateAsync(TResource resource);

        /// <summary>
        /// Performs a DELETE request against the API.
        /// </summary>
        /// <param name="identifier">The resource to be deleted.</param>
        Task<IApiResponse> DeleteAsync(TIdentifier identifier);

        /// <summary>
        /// Performs a PUT request against the API.
        /// </summary>
        /// <param name="resource">The resource to be replaced.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<IApiResponse<TResource>> ReplaceAsync(TResource resource);

        /// <summary>
        /// Performs a PATCH request against the API.
        /// </summary>
        /// <param name="resource">The resource to be updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Task<IApiResponse<TResource>> UpdateAsync(TResource resource);
    }
}
