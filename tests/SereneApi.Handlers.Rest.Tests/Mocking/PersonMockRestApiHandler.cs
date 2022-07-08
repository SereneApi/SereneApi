using SereneApi.Extensions.Mocking.Rest.Handler;
using SereneApi.Extensions.Mocking.Rest.Handler.Attributes;
using SereneApi.Handlers.Rest.Tests.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Tests.Mocking
{
    internal class PersonMockRestApiHandler : MockRestApiHandlerBase
    {
        private List<MockPersonDto> _persons = new List<MockPersonDto>
        {
            new MockPersonDto
            {
                Age = 18,
                BirthDate = new DateTime(1997, 05, 23),
                Name = "John Smith"
            },
            new MockPersonDto
            {
                Age = 13,
                BirthDate = new DateTime(2002, 11, 07),
                Name = "Hans Junior"
            },
            new MockPersonDto
            {
                Age = 25,
                BirthDate = new DateTime(1990, 02, 15),
                Name = "Jessica Batting"
            },
        };

        [MockGet("ByAge/{age}")]
        public Task<IMockResult> GetByAgeAsync(int age)
        {
            return Task.FromResult(Ok(_persons.Where(p => p.Age == age)));
        }

        [IsDelayed(2000)]
        [MockGet]
        public Task<IMockResult> GetAsync()
        {
            return Task.FromResult(Ok(_persons));
        }

        [MockPost("Create")]
        public IMockResult Create([BindBody] MockPersonDto person)
        {
            if (person != null)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
