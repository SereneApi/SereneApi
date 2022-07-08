using SereneApi.Extensions.Mocking.Rest.Configuration.Settings;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Delays a response by the specified amount of time.
    /// <remarks>Can be applied to a Mock HttpMethod or the Mock Handler, if applied to both the HttpMethod attribute supersedes the Handler Attribute.</remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public abstract class DelayedAttribute : Attribute
    {
        public abstract IDelaySettings GetDelaySettings();
    }
}
