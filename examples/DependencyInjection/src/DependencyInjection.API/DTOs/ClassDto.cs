using System.Collections.Generic;

namespace DependencyInjection.API.DTOs
{
    public class ClassDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }

        public static ClassDto ClassA { get; } = new ClassDto
        {
            Id = 0,
            Name = "Class Alpha",
            State = "State in a place"
        };

        public static ClassDto ClassB { get; } = new ClassDto
        {
            Id = 1,
            Name = "Class Beta",
            State = "Who knows"
        };

        public static List<ClassDto> Classes { get; } = new List<ClassDto>
        {
            ClassA,
            ClassB
        };
    }
}
