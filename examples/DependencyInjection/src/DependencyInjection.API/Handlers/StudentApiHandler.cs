﻿using DependencyInjection.API.DTOs;
using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Http.Responses;
using SereneApi.Handlers.Rest;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DependencyInjection.API.Handlers
{
    public class StudentApiHandler : RestApiHandler, IStudentApi
    {
        // This is important for Dependency Injection to work! The Handler interface must be set as
        // the generic for IApiHandlerOptions. This is required so AspNet gets the right settings
        // for the current handler.
        public StudentApiHandler(IApiSettings<StudentApiHandler> settings) : base(settings)
        {
        }

        public Task<IApiResponse> CreateAsync(StudentDto student)
        {
            // The StudentDto value will be passed to JSON and sent in the body of the request http://localhost:8080/api/Students

            return MakeRequest
                .UsingMethod(HttpMethod.Post)
                .WithInBodyContent(student)
                .ExecuteAsync();
        }

        public Task<IApiResponse<List<StudentDto>>> FindByGivenAndLastName(StudentDto student)
        {
            // In this example, only the Given and Last name values will used for the query. http://localhost:8080/api/Students?GivenName=value&LastName=value
            return MakeRequest
                .UsingMethod(HttpMethod.Get)
                .AgainstEndpoint("SearchBy/GivenAndLastName")
                .WithQuery(student, s => new { s.GivenName, s.LastName })
                .RespondsWith<List<StudentDto>>()
                .ExecuteAsync();
        }

        public Task<IApiResponse<List<StudentDto>>> GetAllAsync()
        {
            // This is a simple GET request with no endpoint or parameters provided. http://localhost:8080/api/Students
            return MakeRequest
                .UsingMethod(HttpMethod.Get)
                .RespondsWith<List<StudentDto>>()
                .ExecuteAsync();
        }

        public Task<IApiResponse<StudentDto>> GetAsync(long studentId)
        {
            // This GET request will use the students Id as a parameter for the request. http://localhost:8080/api/Students/{studentId}
            return MakeRequest
                .UsingMethod(HttpMethod.Get)
                .WithParameter(studentId)
                .RespondsWith<StudentDto>()
                .ExecuteAsync();
        }

        public Task<IApiResponse<List<ClassDto>>> GetStudentClassesAsync(long studentId)
        {
            // Here we are using an Endpoint Template, allowing more complex APIs. http://localhost:8080/api/Students/{studentId}/Classes
            return MakeRequest
                .UsingMethod(HttpMethod.Get)
                .AgainstEndpoint("{0}/Classes")
                .WithParameter(studentId)
                .RespondsWith<List<ClassDto>>()
                .ExecuteAsync();
        }
    }
}