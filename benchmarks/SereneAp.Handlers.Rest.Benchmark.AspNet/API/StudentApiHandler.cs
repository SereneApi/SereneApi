using SereneApi.Benchmark.AspNet.API;
using SereneApi.Core.Options;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using SereneApi.Handlers.Rest;
using System.Threading.Tasks;

namespace SereneAp.Handlers.Rest.Benchmark.AspNet.API
{
    public class StudentApiHandler : RestApiHandler, IStudentApi
    {
        public StudentApiHandler(IApiOptions<StudentApiHandler> options) : base(options)
        {
        }

        public Task<IApiResponse<StudentDto>> GetStudents()
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWith<StudentDto>()
                .ExecuteAsync();
        }
    }
}
