using DependencyInjection.API;
using DependencyInjection.API.DTOs;
using SereneApi;
using SereneApi.Abstractions;
using SereneApi.DependencyInjection;
using SereneApi.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DependencyInjection.WebUi.Handlers
{
    public class StudentApiHandler : ApiHandler, IStudentApi
    {
        // This is important for Dependency Injection to work!
        // The Handler name must be set as the generic.
        // This is required so AspNet gets the right settings for the current handler.
        public StudentApiHandler(IApiHandlerOptions<StudentApiHandler> options) : base(options)
        {
        }

        public Task<IApiResponse<StudentDto>> GetAsync(long studentId)
        {
            // This GET request will use the students Id as a parameter for the request.
            return InPathRequestAsync<StudentDto>(ApiMethod.Get, studentId);
        }

        public Task<IApiResponse<List<StudentDto>>> GetAllAsync()
        {
            // This is a simple GET request with no endpoint or parameters provided.
            return InPathRequestAsync<List<StudentDto>>(ApiMethod.Get);
        }

        public Task<IApiResponse<List<StudentDto>>> FindByGivenAndLastName(StudentDto student)
        {
            // In this example, only the Given and Last name values will used for the query.
            // The query created would be ?GivenName=value&LastName=value
            return InPathRequestWithQueryAsync<List<StudentDto>, StudentDto>(ApiMethod.Get, student,
                s => new { s.GivenName, s.LastName });
        }

        public Task<IApiResponse> CreateAsync(StudentDto student)
        {
            return InBodyRequestAsync(ApiMethod.Post, student);
        }

        public Task<IApiResponse<List<ClassDto>>> GetStudentClasses(long studentId)
        {
            // Here we are using an Endpoint Template, allowing more complex APIs.
            // In the scenario below, the {0} will be replaced with the students ID.
            return InPathRequestAsync<List<ClassDto>>(ApiMethod.Get, "{0}/Classes", studentId);
        }
    }
}
