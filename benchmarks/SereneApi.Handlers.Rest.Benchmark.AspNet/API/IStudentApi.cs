using ApiCommon.Core.Responses;
using System;
using System.Threading.Tasks;

namespace ApiCommon.Handlers.Rest.Benchmark.AspNet.API
{
    public interface IStudentApi : IDisposable
    {
        Task<IApiResponse<StudentDto>> GetStudents();
    }
}