using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Handler;

namespace SereneApi.Core.Configuration.Settings
{
    public class ApiSettings<TApiHandler> : ApiSettings, IApiSettings<TApiHandler> where TApiHandler : IApiHandler
    {
        public ApiSettings(IDependencyProvider dependencies) : base(dependencies)
        {
        }
    }
}