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
            return MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWithType<StudentDto>()
                .ExecuteAsync();
        }
    }
}
