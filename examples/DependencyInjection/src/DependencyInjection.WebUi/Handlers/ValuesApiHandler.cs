using DependencyInjection.API;
using SereneApi;
using SereneApi.Abstractions;
using SereneApi.Abstractions.Handler;
using System.Threading.Tasks;

namespace DependencyInjection.WebUi.Handlers
{
    public class ValuesApiHandler: ApiHandler, IValuesApi
    {
        public ValuesApiHandler(IApiHandlerOptions<IValuesApi> options) : base(options)
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
