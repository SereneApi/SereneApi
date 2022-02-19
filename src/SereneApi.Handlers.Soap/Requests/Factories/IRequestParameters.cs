using System.Collections.Generic;

namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IRequestParameters : IResponseType
    {
        IResponseType WithParameters(Dictionary<string, object> parameters);

        IResponseType WithParameters(Dictionary<string, string> parameters);

        IResponseType WithParameters<TParam>(TParam parameters) where TParam : class;
    }
}