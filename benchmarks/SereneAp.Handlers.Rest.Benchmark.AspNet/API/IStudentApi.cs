using SereneApi.Benchmark.AspNet.API;
using SereneApi.Core.Responses;
using System;
using System.Threading.Tasks;

namespace SereneAp.Handlers.Rest.Benchmark.AspNet.API
{
    public interface IStudentApi : IDisposable
    {
        Task<IApiResponse<StudentDto>> GetStudents();
    }
}
