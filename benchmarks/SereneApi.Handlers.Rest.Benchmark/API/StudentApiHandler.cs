using ApiCommon.Core.Options;
using ApiCommon.Core.Requests;
using ApiCommon.Core.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiCommon.Handlers.Rest.Benchmark.API
{
    public class StudentApiHandler : RestApiHandler, IStudentApi
    {
        public StudentApiHandler(IApiOptions options) : base(options)
        {
        }

        public Task<IApiResponse<List<StudentDto>>> GetStudentsAsync()
        {
            return MakeRequest
                .UsingMethod(Method.Get)
                .AgainstVersion("V2")
                .RespondsWith<List<StudentDto>>()
                .ExecuteAsync();
        }
    }
}