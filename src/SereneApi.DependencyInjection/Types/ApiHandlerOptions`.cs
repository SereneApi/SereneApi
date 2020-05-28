using System;
using SereneApi.Interfaces;
using SereneApi.Types;

namespace SereneApi.DependencyInjection.Types
{
    /// <summary>
    /// The <see cref="ApiHandlerOptions{TApiHandler}"/> to be used by the <see cref="ApiHandler"/> when making API requests
    /// </summary>
    public class ApiHandlerOptions<TApiHandler> : ApiHandlerOptions where TApiHandler : ApiHandler
    {
        #region Public Properties

        /// <summary>
        /// The <see cref="Type"/> of <see cref="ApiHandler"/> these <see cref="Interfaces.IApiHandlerOptions"/> will be used for.
        /// </summary>
        public Type HandlerType => typeof(TApiHandler);

        #endregion

        public ApiHandlerOptions(IDependencyCollection dependencyCollection, Uri source, string resource) : base(dependencyCollection, source, resource)
        {
        }
    }
}
