using System;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Factories
{
    public interface IMockResponseContent : IMockResponseData
    {
        /// <summary>
        /// The request must match the specified content for this Mock Response to reply to it.
        /// </summary>
        /// <typeparam name="TContent">The content <see cref="Type"/>.</typeparam>
        /// <param name="content">The content to be matched against the request.</param>
        IMockResponseData ForContent<TContent>(TContent content);
    }
}