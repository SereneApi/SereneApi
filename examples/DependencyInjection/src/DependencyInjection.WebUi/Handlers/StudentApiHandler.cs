using DependencyInjection.API;
using DependencyInjection.API.DTOs;
using SereneApi;
using SereneApi.Extensions.DependencyInjection.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DependencyInjection.WebUi.Handlers
{
    public class StudentApiHandler: ApiHandler, IStudentApi
    {
        // This is important for Dependency Injection to work!
        // The Handler name must be set as the generic.
        // This is required so AspNet gets the right options for the current handler.
        public StudentApiHandler(IApiHandlerOptions<StudentApiHandler> options) : base(options)
        {
        }

        public Task<IApiResponse<StudentDto>> GetAsync(long studentId)
        {
            // This GET request will use the students Id as a parameter for the request.
            // http://localhost:8080/api/Students/{studentId}
            return PerformRequestAsync<StudentDto>(Method.Get, r => r
                .WithEndPoint(studentId));
        }

        public Task<IApiResponse<List<StudentDto>>> GetAllAsync()
        {
            // This is a simple GET request with no endpoint or parameters provided.
            // http://localhost:8080/api/Students
            return PerformRequestAsync<List<StudentDto>>(Method.Get);
        }

        public Task<IApiResponse<List<StudentDto>>> FindByGivenAndLastName(StudentDto student)
        {
            // In this example, only the Given and Last name values will used for the query.
            // http://localhost:8080/api/Students?GivenName=value&LastName=value
            return PerformRequestAsync<List<StudentDto>>(Method.Get, r => r
                .WithQuery(student, s => new { s.GivenName, s.LastName }));
        }

        public Task<IApiResponse> CreateAsync(StudentDto student)
        {
            // The StudentDto value will be passed to JSON and sent in the body of the request
            // http://localhost:8080/api/Students
            return PerformRequestAsync(Method.Post, r => r
                .WithInBodyContent(student));
        }

        public Task<IApiResponse<List<ClassDto>>> GetStudentClasses(long studentId)
        {
            // Here we are using an Endpoint Template, allowing more complex APIs.
            // http://localhost:8080/api/Students/{studentId}/Classes
            return PerformRequestAsync<List<ClassDto>>(Method.Get, r => r
                .WithEndPointTemplate("{0}/Classes", studentId));
        }
    }
}
