using DeltaWare.SereneApi.Enums;
using System;

namespace DeltaWare.SereneApi.Interfaces
{
    public interface IDependency
    {
        DependencyBinding Binding { get; }

        Type Type { get; }
    }
}
