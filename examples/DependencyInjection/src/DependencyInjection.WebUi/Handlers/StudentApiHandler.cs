using DependencyInjection.API;
using DependencyInjection.API.DTOs;
using SereneApi;
using SereneApi.Abstractions;
using SereneApi.Abstractions.Handler;
using System.Collections.Generic;
using System.Threading.Tasks;
using SereneApi.Abstractions.Handler.Options;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Responses;

namespace DependencyInjection.WebUi.Handlers
{
    public class StudentApiHandler: ApiHandler, IStudentApi
    {
        // This is important for Dependency Injection to work!
        // The Handler interface must be set as the generic for IApiHandlerOptions.
        // This is required so AspNet gets the right options for the current handler.
        public StudentApiHandler(IOptions<IStudentApi> options) : base(options)
        {
        }

        public Task<IApiResponse<StudentDto>> GetAsync(long studentId)
        {
            // This GET request will use the students Id as a parameter for the request.
            // http://localhost:8080/api/Students/{studentId}
            return PerformRequestAsync<StudentDto>(Method.GET, r => r
                .WithEndPoint(studentId));
        }

        public Task<IApiResponse<List<StudentDto>>> GetAllAsync()
        {
            // This is a simple GET request with no endpoint or parameters provided.
            // http://localhost:8080/api/Students
            return PerformRequestAsync<List<StudentDto>>(Method.GET);
        }

        public Task<IApiResponse<List<StudentDto>>> FindByGivenAndLastName(StudentDto student)
        {
            // In this example, only the Given and Last name values will used for the query.
            // http://localhost:8080/api/Students?GivenName=value&LastName=value
            return PerformRequestAsync<List<StudentDto>>(Method.GET, r => r
                .WithEndPoint("SearchBy/GivenAndLastName")
                .WithQuery(student, s => new { s.GivenName, s.LastName }));
        }

        public Task<IApiResponse> CreateAsync(StudentDto student)
        {
            // The StudentDto value will be passed to JSON and sent in the body of the request
            // http://localhost:8080/api/Students
            return PerformRequestAsync(Method.POST, r => r
                .WithInBodyContent(student));
        }

        public Task<IApiResponse<List<ClassDto>>> GetStudentClassesAsync(long studentId)
        {
            // Here we are using an Endpoint Template, allowing more complex APIs.
            // http://localhost:8080/api/Students/{studentId}/Classes
            return PerformRequestAsync<List<ClassDto>>(Method.GET, r => r
                .WithEndPointTemplate("{0}/Classes", studentId));
        }
    }
}
