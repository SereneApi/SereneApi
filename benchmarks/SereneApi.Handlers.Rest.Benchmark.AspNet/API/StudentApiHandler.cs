using SereneApi.Core.Http.Responses;
using SereneApi.Core.Requests;
using System.Threading.Tasks;
using SereneApi.Core.Configuration.Settings;

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
                .UsingMethod(Method.Get)
                .RespondsWith<StudentDto>()
                .ExecuteAsync();
        }
    }
}