using SereneApi.Abstractions.Factories;

namespace SereneApi.Abstractions.Configuration
{
    public interface IConfiguration
    {
        IApiHandlerConfiguration ApiHandler { get; }
    }
}
