using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Http.Responses;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Benchmark.AspNet.API
{
    public class StudentApiHandler : RestApiHandler, IStudentApi
    {
        public StudentApiHandler(IApiSettings<StudentApiHandler> options) : base(options)
        {
        }

        public Task<IApiResponse<StudentDto>> GetStudents()
        {
            return MakeRequest
                .UsingMethod(HttpMethod.Get)
                .RespondsWith<StudentDto>()
                .ExecuteAsync();
        }
    }
}