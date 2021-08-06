using DependencyInjection.API;
using SereneApi;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response;
using System.Threading.Tasks;

namespace DependencyInjection.WebUi.Handlers
{
    public class ValuesApiHandler : BaseApiHandler, IValuesApi
    {
        public ValuesApiHandler(IApiOptions<IValuesApi> options) : base(options)
        {
        }

        public Task<IApiResponse<int>> GetAsync(int value)
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .AgainstEndpoint("int/{0}")
                .WithParameter(value)
                .RespondsWithType<int>()
                .ExecuteAsync();
        }

        public Task<IApiResponse<string>> GetAsync(string value)
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .AgainstEndpoint("string/{0}")
                .WithParameter(value)
                .RespondsWithType<string>()
                .ExecuteAsync();
        }
    }
}
