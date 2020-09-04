using DependencyInjection.API;
using SereneApi;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;
using System.Threading.Tasks;

namespace DependencyInjection.WebUi.Handlers
{
    public class ValuesApiHandler: BaseApiHandler, IValuesApi
    {
        public ValuesApiHandler(IApiOptions<IValuesApi> options) : base(options)
        {
        }

        public Task<IApiResponse<int>> GetAsync(int value)
        {
            return PerformRequestAsync<int>(Method.GET,
                r => r.WithEndPoint("int/{0}").WithParameters(value));
        }

        public IApiResponse<string> GetAsync(string value)
        {
            return PerformRequest<string>(Method.GET,
                r => r.WithEndPoint("string/{0}").WithParameters(value));
        }
    }
}
