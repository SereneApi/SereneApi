using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjection.Service.Dtos
{
    public class UserDto
    {
        public long Id { get; sdet; }

        public string GivenName { get; set; }

        public string lastName { get; set; }

        public string Email { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
