using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Requests;
using SereneApi.Handlers.Rest;
using System.IO;
using System.Threading.Tasks;

namespace DependencyInjection.API.Handlers
{
    public class ValuesApiHandler : RestApiHandler, IValuesApi
    {
        public ValuesApiHandler(IApiSettings<ValuesApiHandler> settings) : base(settings)
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

        public Task<IApiResponse<MemoryStream>> GetSamplePfgAsync()
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .AgainstResource("GetSamplePdf")
                .RespondsWith<MemoryStream>()
                .ExecuteAsync();
        }
    }
}