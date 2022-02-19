using System;
using System.Collections.Generic;

namespace SereneApi.Handlers.Rest.Tests.Mock
{
    public class MockPersonDto
    {
        private static readonly MockPersonDto _benJerry = new()
        {
            Age = 16,
            Name = "Ben Jerry",
            BirthDate = new DateTime(2002, 08, 20)
        };

        private static readonly MockPersonDto _johnSmith = new()
        {
            Age = 18,
            Name = "John Smith",
            BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
        };

        public static List<MockPersonDto> All { get; } = new()
        {
            _benJerry,
            _johnSmith
        };

        public static MockPersonDto BenJerry => _benJerry;

        public static MockPersonDto JohnSmith => _johnSmith;

        public int Age { get; set; }

        public DateTime BirthDate { get; set; }
        public string Name { get; set; }
    }
}