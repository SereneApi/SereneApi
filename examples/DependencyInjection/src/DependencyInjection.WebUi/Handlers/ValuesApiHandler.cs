using DependencyInjection.API;
using SereneApi;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using System.Threading.Tasks;

namespace DependencyInjection.WebUi.Handlers
{
    public class ValuesApiHandler: ApiHandler, IValuesApi
    {
        public ValuesApiHandler(IApiHandlerOptions<ValuesApiHandler> options) : base(options)
        {
        }

        public Task<IApiResponse<int>> GetAsync(int value)
        {
            return PerformRequestAsync<int>(Method.Get, 
                r => r.WithEndPointTemplate("int/{0}", value));
        }

        public IApiResponse<string> GetAsync(string value)
        {
            return PerformRequest<string>(Method.Get, 
                r => r.WithEndPointTemplate("string/{0}", value));
        }
    }
}
