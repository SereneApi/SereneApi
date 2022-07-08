using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Http.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest
{
    /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}"/>
    public abstract class CrudApiHandler<TResource, TIdentifier> : RestApiHandler, ICrudApi<TResource, TIdentifier> where TResource : class where TIdentifier : struct
    {
        /// <summary>
        /// Instantiates a new Instance of the <see cref="CrudApiHandler{TResource,TIdentifier}"/>
        /// </summary>
        /// <param name="settings></param>
        protected CrudApiHandler(IApiSettings settings) : base(settings)
        {
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.CreateAsync"/>
        public virtual Task<IApiResponse<TResource>> CreateAsync(TResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            return MakeRequest
                .UsingMethod(HttpMethod.Post)
                .WithInBodyContent(resource)
                .RespondsWith<TResource>()
                .ExecuteAsync();
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.DeleteAsync"/>
        public virtual Task<IApiResponse> DeleteAsync(TIdentifier identifier)
        {
            return MakeRequest
                .UsingMethod(HttpMethod.Delete)
                .WithParameter(identifier)
                .ExecuteAsync();
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.GetAsync(TIdentifier)"/>
        public virtual Task<IApiResponse<TResource>> GetAsync(TIdentifier identifier)
        {
            return MakeRequest
                .UsingMethod(HttpMethod.Get)
                .WithParameter(identifier)
                .RespondsWith<TResource>()
                .ExecuteAsync();
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.GetAsync()"/>
        public virtual Task<IApiResponse<List<TResource>>> GetAsync()
        {
            return MakeRequest
                .UsingMethod(HttpMethod.Get)
                .RespondsWith<List<TResource>>()
                .ExecuteAsync();
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.ReplaceAsync"/>
        public virtual Task<IApiResponse<TResource>> ReplaceAsync(TResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            return MakeRequest
                .UsingMethod(HttpMethod.Put)
                .WithInBodyContent(resource)
                .RespondsWith<TResource>()
                .ExecuteAsync();
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.UpdateAsync"/>
        public virtual Task<IApiResponse<TResource>> UpdateAsync(TResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            return MakeRequest
                .UsingMethod(HttpMethod.Patch)
                .WithInBodyContent(resource)
                .RespondsWith<TResource>()
                .ExecuteAsync();
        }
    }
}