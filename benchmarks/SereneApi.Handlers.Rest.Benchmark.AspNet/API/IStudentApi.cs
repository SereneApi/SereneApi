using SereneApi.Core.Http.Responses;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Benchmark.AspNet.API
{
    public interface IStudentApi
    {
        Task<IApiResponse<StudentDto>> GetStudents();
    }
}