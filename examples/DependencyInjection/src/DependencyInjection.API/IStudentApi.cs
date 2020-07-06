using DependencyInjection.API.DTOs;
using SereneApi.Abstractions.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DependencyInjection.API
{
    public interface IStudentApi
    {
        Task<IApiResponse<StudentDto>> GetAsync(long studentId);

        Task<IApiResponse<List<StudentDto>>> GetAllAsync();

        Task<IApiResponse<List<StudentDto>>> FindByGivenAndLastName(StudentDto student);

        Task<IApiResponse> CreateAsync(StudentDto student);

        Task<IApiResponse<List<ClassDto>>> GetStudentClassesAsync(long studentId);
    }
}
