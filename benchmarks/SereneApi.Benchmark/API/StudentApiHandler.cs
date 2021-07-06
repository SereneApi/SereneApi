using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response;
using System.Threading.Tasks;

namespace SereneApi.Benchmark.API
{
    public class StudentApiHandler : BaseApiHandler, IStudentApi
    {
        public StudentApiHandler(IApiOptions options) : base(options)
        {
        }

        public Task<IApiResponse<StudentDto>> GetStudentsAsync()
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWithType<StudentDto>()
                .ExecuteAsync();
        }
    }
}
