using System.Collections.Generic;

namespace SereneApi.Handlers.Soap.Parameters
{
    public interface IParameterService
    {
        Dictionary<string, string> GetParameters<TParam>(TParam parameters) where TParam : class;
    }
}
