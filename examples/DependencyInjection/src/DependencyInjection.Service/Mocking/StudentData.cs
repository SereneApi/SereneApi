using DependencyInjection.API.DTOs;
using System;
using System.Collections.Generic;

namespace DependencyInjection.Service.Mocking
{
    public static class StudentData
    {
        public static StudentDto StudentA { get; } = new StudentDto
        {
            BirthDate = new DateTime(2000, 10, 05),
            Email = "John.Smith@someschool.edu",
            GivenName = "John",
            LastName = "Smith",
            Id = 0
        };

        public static List<StudentDto> Students { get; } = new List<StudentDto>
        {
            StudentA
        };
    }
}
