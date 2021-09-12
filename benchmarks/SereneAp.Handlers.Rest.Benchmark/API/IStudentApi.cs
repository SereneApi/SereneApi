using SereneApi.Benchmark.API;
using SereneApi.Core.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SereneAp.Handlers.Rest.Benchmark.API
{
    public interface IStudentApi : IDisposable
    {
        Task<IApiResponse<List<StudentDto>>> GetStudentsAsync();
    }
}