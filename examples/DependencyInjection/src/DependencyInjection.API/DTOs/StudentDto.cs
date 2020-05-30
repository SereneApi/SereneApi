using System;

namespace DependencyInjection.API.DTOs
{
    public class StudentDto
    {
        public long Id { get; set; }

        public string GivenName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
