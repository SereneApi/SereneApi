using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Options.Factories;

namespace SereneApi.Extensions.Profiling
{
    public class Profiler
    {
        public void Attach(IApiOptionsFactory optionsFactory)
        {
            ApiOptionsFactory factory = (ApiOptionsFactory)optionsFactory;

            IDependencyProvider provider = factory.Dependencies.BuildProvider();
        }
    }
}