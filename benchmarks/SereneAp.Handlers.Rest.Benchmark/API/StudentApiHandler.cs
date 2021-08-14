using SereneApi.Benchmark.API;
using SereneApi.Core.Options;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using SereneApi.Handlers.Rest;
using System.Threading.Tasks;

namespace SereneAp.Handlers.Rest.Benchmark.API
{
    public class StudentApiHandler : RestApiHandler, IStudentApi
    {
        public StudentApiHandler(IApiOptions options) : base(options)
        {
        }

        public Task<IApiResponse<StudentDto>> GetStudentsAsync()
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWith<StudentDto>()
                .ExecuteAsync();
        }
    }
}
