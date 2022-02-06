using SereneApi.Core.Handler;
using DeltaWare.Dependencies.Abstractions;

namespace SereneApi.Core.Configuration.Settings
{
    public class ApiSettings<TApiHandler> : ApiSettings, IApiSettings<TApiHandler> where TApiHandler : IApiHandler
    {
        public ApiSettings(IDependencyProvider dependencies) : base(dependencies)
        {
        }
    }
}