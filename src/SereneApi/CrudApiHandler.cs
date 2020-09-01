using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi
{
    /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}"/>
    public abstract class CrudApiHandler<TResource, TIdentifier>: BaseApiHandler, ICrudApi<TResource, TIdentifier> where TResource : class where TIdentifier : struct
    {
        /// <summary>
        /// Instantiates a new Instance of the <see cref="CrudApiHandler{TResource,TIdentifier}"/>
        /// </summary>
        /// <param name="options"></param>
        protected CrudApiHandler([NotNull] IApiOptions options) : base(options)
        {
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.GetAsync"/>
        public virtual Task<IApiResponse<TResource>> GetAsync(TIdentifier identifier)
        {
            return PerformRequestAsync<TResource>(Method.GET, request => request
                .WithEndpoint(identifier));
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.GetAllAsync"/>
        public virtual Task<IApiResponse<List<TResource>>> GetAllAsync()
        {
            return PerformRequestAsync<List<TResource>>(Method.GET);
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.CreateAsync"/>
        public virtual Task<IApiResponse<TResource>> CreateAsync([NotNull] TResource resource)
        {
            if(resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            return PerformRequestAsync<TResource>(Method.POST, request => request
                .WithInBodyContent(resource));
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.DeleteAsync"/>
        public virtual Task<IApiResponse> DeleteAsync(TIdentifier identifier)
        {
            return PerformRequestAsync(Method.DELETE, request => request
                .WithEndpoint(identifier));
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.ReplaceAsync"/>
        public virtual Task<IApiResponse<TResource>> ReplaceAsync([NotNull] TResource resource)
        {
            if(resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            return PerformRequestAsync<TResource>(Method.PUT, request => request
                .WithInBodyContent(resource));
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.UpdateAsync"/>
        public virtual Task<IApiResponse<TResource>> UpdateAsync([NotNull] TResource resource)
        {
            if(resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            return PerformRequestAsync<TResource>(Method.PATCH, request => request
                .WithInBodyContent(resource));
        }
    }
}
