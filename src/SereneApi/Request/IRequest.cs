using SereneApi.Abstractions.Options;
using System;

namespace SereneApi.Request
{
    public interface IRequest: IRequestEndPoint
    {
        /// <summary>
        /// If no resource was provided in the <see cref="IApiOptions"/> to the <see cref="ApiHandler"/> it can be provided here.
        /// </summary>
        /// <param name="resource">The resource to make the request against.</param>
        /// <exception cref="MethodAccessException">Thrown if this method is called when a resource was provided in the <see cref="IApiOptions"/>.</exception>
        IRequestEndPoint AgainstResource(string resource);
    }
}
