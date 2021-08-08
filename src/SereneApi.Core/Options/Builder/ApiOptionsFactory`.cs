using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Handler;
using System;

namespace SereneApi.Core.Options.Builder
{
    public class ApiOptionsFactory<TApiHandler> : IApiOptionsFactory<TApiHandler> where TApiHandler : IApiHandler
    {
        private IDependencyCollection _dependency;

        public ApiOptionsFactory(IDependencyCollection dependency)
        {
            _dependency = dependency;
        }

        public IApiOptions<TApiHandler> BuildOptions()
        {
            throw new NotImplementedException();
        }
    }
}
