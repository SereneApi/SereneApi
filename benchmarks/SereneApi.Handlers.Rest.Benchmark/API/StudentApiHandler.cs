using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Benchmark.API
{
    public class StudentApiHandler : RestApiHandler, IStudentApi
    {
        public StudentApiHandler(IApiSettings<StudentApiHandler> options) : base(options)
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