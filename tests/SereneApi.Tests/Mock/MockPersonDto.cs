using System;

namespace SereneApi.Tests.Mock
{
    public class MockPersonDto
    {
        public int Age { get; set; }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public static MockPersonDto John { get; } = new MockPersonDto
        {
            Age = 18,
            Name = "John Smith",
            BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
        };
    }
}
