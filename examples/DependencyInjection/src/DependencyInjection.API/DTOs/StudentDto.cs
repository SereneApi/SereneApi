using System;

namespace DependencyInjection.API.DTOs
{
    public class StudentDto
    {
        public static StudentDto JohnSmith { get; } = new StudentDto
        {
            Id = 0,
            GivenName = "John",
            LastName = "Smith",
            Email = "John.Smith@SomeSchool.com",
            BirthDate = new DateTime(2005, 06, 13)
        };

        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string GivenName { get; set; }
        public long Id { get; set; }
        public string LastName { get; set; }
    }
}