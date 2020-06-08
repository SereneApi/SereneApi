using System.Collections.Generic;
using System.Threading.Tasks;

namespace SereneApi.Interfaces
{
    /// <summary>
    /// When Inherited; provides the necessary methods for implementing RESTful CRUD Api consumer
    /// </summary>
    /// <typeparam name="TResource">The resource the CRUD Api will be acting on, both send and receive.</typeparam>
    /// <typeparam name="TIdentifier">The Identifier used by the resource, this could be a Guid, long or int.</typeparam>
    public interface ICrudApi<TResource, in TIdentifier> where TResource : class where TIdentifier : struct
    {
        /// <summary>
        /// Performs a GET request using the <see cref="TIdentifier"/> as the Resource Identity
        /// </summary>
        /// <param name="identity">The Identity of the Resource</param>
        Task<IApiResponse<TResource>> GetAsync(TIdentifier identity);

        /// <summary>
        /// Performs a GET request returning all <see cref="TResource"/>s
        /// </summary>
        Task<IApiResponse<IList<TResource>>> GetAllAsync();

        /// <summary>
        /// Performs a POST request creating a <see cref="TResource"/>
        /// </summary>
        /// <param name="resource">The <inheritdoc cref="TResource"/> to be created</param>
        Task<IApiResponse<TResource>> CreateAsync(TResource resource);

        /// <summary>
        /// Performs a DELETE request deleting the <see cref="TResource"/>
        /// </summary>
        /// <param name="identifier">The <see cref="TIdentifier"/> to be deleted</param>
        /// <returns></returns>
        Task<IApiResponse> DeleteAsync(TIdentifier identifier);

        /// <summary>
        /// Performs a PUT request replacing the <see cref="TResource"/>
        /// </summary>
        /// <param name="resource">The <see cref="TResource"/> to be replaced</param>
        /// <returns></returns>
        Task<IApiResponse<TResource>> ReplaceAsync(TResource resource);

        /// <summary>
        /// Performs a PATCH request updating the <see cref="TResource"/>
        /// </summary>
        /// <param name="resource">The <see cref="TResource"/> to be updated</param>
        /// <returns></returns>
        Task<IApiResponse<TResource>> UpdateAsync(TResource resource);
    }
}
