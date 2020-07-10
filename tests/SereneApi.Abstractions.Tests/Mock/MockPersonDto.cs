using System;
using System.Collections.Generic;

namespace SereneApi.Abstractions.Tests.Mock
{
    public class MockPersonDto
    {
        public int Age { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public static MockPersonDto JohnSmith { get; } = new MockPersonDto
        {
            Age = 18,
            Name = "John Smith",
            BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
        };

        public static MockPersonDto BenJerry { get; } = new MockPersonDto
        {
            Age = 16,
            Name = "Ben Jerry",
            BirthDate = new DateTime(2002, 08, 20)
        };

        public static List<MockPersonDto> All { get; } = new List<MockPersonDto>
        {
            BenJerry,
            JohnSmith
        };
    }
}
