using System;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Factories
{
    public interface IMockResponseEndpoint : IMockResponseContent
    {
        /// <summary>
        /// The request must match the specified endpoints for this Mock Response to reply to it.
        /// </summary>
        /// <remarks>Adding the full URL is not necessary. Only the endpoint should be provided. If the full URL is added it will be trimmed.</remarks>
        /// <param name="endpoints">The endpoints to matched against the request.</param>
        IMockResponseContent ForEndpoints(params string[] endpoints);

        /// <summary>
        /// The request must match the specified endpoints for this Mock Response to reply to it.
        /// </summary>
        /// <remarks>Adding the full URL is not necessary. Only the endpoint should be provided. If the full URL is added it will be trimmed.</remarks>
        /// <param name="endpoints">The endpoints to matched against the request.</param>
        IMockResponseContent ForEndpoints(params Uri[] endpoints);
    }
}