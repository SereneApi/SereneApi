using System.Collections.Generic;

namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IApiRequestParameters : IApiResponseType
    {
        IApiResponseType WithParameters(Dictionary<string, object> parameters);

        IApiResponseType WithParameters<TParam>(TParam parameters) where TParam : class;
    }
}
