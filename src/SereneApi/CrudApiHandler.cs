using Microsoft.Extensions.Logging;
using SereneApi.Abstraction;
using SereneApi.Enums;
using SereneApi.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SereneApi
{
    /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}"/>
    public abstract class CrudApiHandler<TResource, TIdentifier> : ApiHandler, ICrudApi<TResource, TIdentifier> where TResource : class where TIdentifier : struct
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Instantiates a new Instance of the <see cref="CrudApiHandler{TResource,TIdentifier}"/>
        /// </summary>
        /// <param name="options"></param>
        protected CrudApiHandler(IApiHandlerOptions options) : base(options)
        {
            options.Dependencies.TryGetDependency(out _logger);
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.GetAsync"/>
        public Task<IApiResponse<TResource>> GetAsync(TIdentifier identity)
        {
            return InPathRequestAsync<TResource>(Method.Get, identity);
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.GetAllAsync"/>
        public Task<IApiResponse<IList<TResource>>> GetAllAsync()
        {
            return InPathRequestAsync<IList<TResource>>(Method.Get);
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.CreateAsync"/>
        public Task<IApiResponse<TResource>> CreateAsync(TResource resource)
        {
            return InBodyRequestAsync<TResource, TResource>(Method.Post, resource);
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.DeleteAsync"/>
        public Task<IApiResponse> DeleteAsync(TIdentifier identifier)
        {
            return InPathRequestAsync(Method.Delete, identifier);
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.ReplaceAsync"/>
        public Task<IApiResponse<TResource>> ReplaceAsync(TResource resource)
        {
            return InBodyRequestAsync<TResource, TResource>(Method.Put, resource);
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.UpdateAsync"/>
        public Task<IApiResponse<TResource>> UpdateAsync(TResource resource)
        {
            return InBodyRequestAsync<TResource, TResource>(Method.Patch, resource);
        }
    }
}
