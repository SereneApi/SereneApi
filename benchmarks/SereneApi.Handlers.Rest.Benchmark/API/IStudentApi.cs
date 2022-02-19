using SereneApi.Core.Http.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Benchmark.API
{
    public interface IStudentApi : IDisposable
    {
        Task<IApiResponse<List<StudentDto>>> GetStudentsAsync();
    }
}