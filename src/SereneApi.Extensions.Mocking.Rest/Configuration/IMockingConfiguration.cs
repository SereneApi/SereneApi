using SereneApi.Extensions.Mocking.Rest.Configuration.Factories;
using SereneApi.Extensions.Mocking.Rest.Handler;

namespace SereneApi.Extensions.Mocking.Rest.Configuration
{
    public interface IMockingConfiguration
    {
        /// <summary>
        /// Registers a Mock Response.
        /// </summary>
        /// <returns>An <see cref="IMockResponseFactory"/> used to configure the Response.</returns>
        IMockResponseFactory RegisterMockResponse();

        /// <summary>
        /// Registers a <see cref="MockRestApiHandlerBase"/>.
        /// </summary>
        /// <typeparam name="T">The Mock Response Handler to be Registered.</typeparam>
        void RegisterMockingHandler<T>() where T : MockRestApiHandlerBase;
    }
}