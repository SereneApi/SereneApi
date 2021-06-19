using SereneApi.Abstractions.Response;
using System;
using System.Threading.Tasks;

namespace SereneApi.Benchmark.AspNet.API
{
    public interface IStudentApi : IDisposable
    {
        Task<IApiResponse<StudentDto>> GetStudents();
    }
}
