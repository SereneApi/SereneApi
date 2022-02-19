using DependencyInjection.API.DTOs;
using SereneApi.Core.Http.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DependencyInjection.API
{
    public interface IStudentApi
    {
        Task<IApiResponse> CreateAsync(StudentDto student);

        Task<IApiResponse<List<StudentDto>>> FindByGivenAndLastName(StudentDto student);

        Task<IApiResponse<List<StudentDto>>> GetAllAsync();

        Task<IApiResponse<StudentDto>> GetAsync(long studentId);

        Task<IApiResponse<List<ClassDto>>> GetStudentClassesAsync(long studentId);
    }
}