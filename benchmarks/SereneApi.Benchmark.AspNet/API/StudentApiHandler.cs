using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response;
using System.Threading.Tasks;

namespace SereneApi.Benchmark.AspNet.API
{
    public class StudentApiHandler : BaseApiHandler, IStudentApi
    {
        public StudentApiHandler(IApiOptions<IStudentApi> options) : base(options)
        {
        }

        public Task<IApiResponse<StudentDto>> GetStudents()
        {
            return BuildRequest
                .UsingMethod(Method.GET)
                .RespondsWithContent<StudentDto>()
                .ExecuteAsync();
        }
    }
}
