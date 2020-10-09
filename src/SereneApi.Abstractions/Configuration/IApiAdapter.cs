using SereneApi.Abstractions.Events;

namespace SereneApi.Abstractions.Configuration
{
    /// <summary>
    /// Provides services used for connecting services to SereneApi.
    /// </summary>
    /// <remarks>This does not provide any form of API Configuration.</remarks>
    public interface IApiAdapter
    {
        /// <summary>
        /// Subscribe and UnSubscribe events.
        /// </summary>
        IEventRelay EventRelay { get; }
    }
}
