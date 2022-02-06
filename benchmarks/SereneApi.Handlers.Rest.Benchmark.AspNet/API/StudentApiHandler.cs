using ApiCommon.Core.Options;
using ApiCommon.Core.Requests;
using ApiCommon.Core.Responses;
using System.Threading.Tasks;

namespace ApiCommon.Handlers.Rest.Benchmark.AspNet.API
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