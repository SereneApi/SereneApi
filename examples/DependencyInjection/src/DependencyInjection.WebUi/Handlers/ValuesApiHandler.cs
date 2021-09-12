using DependencyInjection.API;
using SereneApi.Core.Options;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using SereneApi.Handlers.Rest;
using System.Threading.Tasks;

namespace DependencyInjection.WebUi.Handlers
{
    public class ValuesApiHandler : RestApiHandler, IValuesApi
    {
        public ValuesApiHandler(IApiOptions<ValuesApiHandler> options) : base(options)
        {
        }

        public Task<IApiResponse<int>> GetAsync(int value)
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .AgainstEndpoint("int/{0}")
                .WithParameter(value)
                .RespondsWith<int>()
                .ExecuteAsync();
        }

        public Task<IApiResponse<string>> GetAsync(string value)
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .AgainstEndpoint("string/{0}")
                .WithParameter(value)
                .RespondsWith<string>()
                .ExecuteAsync();
        }
    }
}