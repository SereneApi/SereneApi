using System;

namespace SereneApi.Adapters.Testing.Profiling.Api
{
    public interface IApiProfile<TApi>: IApiProfile
    {
        /// <summary>
        /// Retrieves usage statics relevant to the specified Endpoint.
        /// </summary>
        /// <param name="nameof">The nameof the Method used to interact with the Endpoint.</param>
        /// <example>ByEndpoint(e => nameof(e.GetFooAsync));</example>
        IEndpointProfile ByEndpoint(Func<TApi, string> endpointName);
    }
}
