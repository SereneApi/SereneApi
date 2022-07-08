using SereneApi.Core.Requests;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Factories
{
    public interface IMockResponseFactory : IMockResponseEndpoint
    {
        /// <summary>
        /// The request must match the specified <see cref="Method"/> for this Mock Response to reply to it.
        /// </summary>
        /// <param name="methods">The methods to be matched against the request.</param>
        IMockResponseEndpoint ForMethod(params Method[] methods);
    }
}