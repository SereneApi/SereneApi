using ApiCommon.Core.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiCommon.Handlers.Rest.Benchmark.API
{
    public interface IStudentApi : IDisposable
    {
        Task<IApiResponse<List<StudentDto>>> GetStudentsAsync();
    }
}