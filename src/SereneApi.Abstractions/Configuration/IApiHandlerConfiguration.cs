using System;
using System.Collections.Generic;
using System.Text;
using DeltaWare.Dependencies;
using SereneApi.Abstractions.Handler.Options;

namespace SereneApi.Abstractions.Configuration
{
    public interface IApiHandlerConfiguration
    {
        IOptionsBuilder GetOptionsBuilder();

        IOptionsBuilder GetOptionsBuilder<TBuilder>() where TBuilder : IOptionsBuilder, new();
    }
}
