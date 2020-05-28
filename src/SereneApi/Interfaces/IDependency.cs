using SereneApi.Enums;
using System;

namespace SereneApi.Interfaces
{
    public interface IDependency
    {
        DependencyBinding Binding { get; }

        Type Type { get; }
    }
}
