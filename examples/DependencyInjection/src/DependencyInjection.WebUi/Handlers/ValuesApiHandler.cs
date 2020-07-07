using DependencyInjection.API;
using SereneApi;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;
using System.Threading.Tasks;
using SereneApi.Abstractions.Options;

namespace DependencyInjection.WebUi.Handlers
{
    public class ValuesApiHandler: ApiHandler, IValuesApi
    {
        public ValuesApiHandler(IApiOptions<IValuesApi> apiOptions) : base(apiOptions)
        {
        }

        public Task<IApiResponse<int>> GetAsync(int value)
        {
            return PerformRequestAsync<int>(Method.GET,
                r => r.WithEndPointTemplate("int/{0}", value));
        }

        public IApiResponse<string> GetAsync(string value)
        {
            return PerformRequest<string>(Method.GET,
                r => r.WithEndPointTemplate("string/{0}", value));
        }
    }
}
