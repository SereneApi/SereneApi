using System;

namespace ApiCommon.Handlers.Rest.Benchmark.API
{
    public class StudentDto
    {
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string GivenName { get; set; }
        public long Id { get; set; }
        public string LastName { get; set; }
    }
}