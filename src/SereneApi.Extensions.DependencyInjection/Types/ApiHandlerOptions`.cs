using SereneApi.Extensions.DependencyInjection.Interfaces;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;

namespace SereneApi.Extensions.DependencyInjection.Types
{
    /// <summary>
    /// The <see cref="ApiHandlerOptions{TApiHandler}"/> to be used by the <see cref="ApiHandler"/> when making API requests
    /// </summary>
    public class ApiHandlerOptions<TApiHandler>: ApiHandlerOptions, IApiHandlerOptions<TApiHandler> where TApiHandler : ApiHandler
    {
        #region Public Properties

        /// <summary>
        /// The <see cref="Type"/> of <see cref="ApiHandler"/> these <see cref="IApiHandlerOptions"/> will be used for.
        /// </summary>
        public Type HandlerType => typeof(TApiHandler);

        #endregion

        public ApiHandlerOptions(IDependencyCollection dependencyCollection, Uri source, string resource, string resourcePath) : base(dependencyCollection, source, resource, resourcePath)
        {
        }
    }
}
