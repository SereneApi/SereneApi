using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SereneApi
{
    /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}"/>
    public abstract class CrudApiHandler<TResource, TIdentifier> : BaseApiHandler, ICrudApi<TResource, TIdentifier> where TResource : class where TIdentifier : struct
    {
        /// <summary>
        /// Instantiates a new Instance of the <see cref="CrudApiHandler{TResource,TIdentifier}"/>
        /// </summary>
        /// <param name="options"></param>
        protected CrudApiHandler(IApiOptions options) : base(options)
        {
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.GetAsync(TIdentifier)"/>
        public virtual Task<IApiResponse<TResource>> GetAsync(TIdentifier identifier)
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .WithParameter(identifier)
                .RespondsWithType<TResource>()
                .ExecuteAsync();
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.GetAsync()"/>
        public virtual Task<IApiResponse<List<TResource>>> GetAsync()
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWithType<List<TResource>>()
                .ExecuteAsync();
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.CreateAsync"/>
        public virtual Task<IApiResponse<TResource>> CreateAsync(TResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            return MakeRequest
                .UsingMethod(Method.Post)
                .AddInBodyContent(resource)
                .RespondsWithType<TResource>()
                .ExecuteAsync();
        }

        /// <inheritdoc cref="ICrudApi{TResource,TIdentifier}.DeleteAsync"/>
        public virtual Task<IApiResponse> DeleteAsync(TIdentifier identifier)
        {
            return MakeRequest
                .UsingMethod(Method.Delete)
                .WithParameter(identifier)
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
                .UsingMethod(Method.Put)
                .AddInBodyContent(resource)
                .RespondsWithType<TResource>()
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
                .UsingMethod(Method.Patch)
                .AddInBodyContent(resource)
                .RespondsWithType<TResource>()
                .ExecuteAsync();
        }
    }
}
