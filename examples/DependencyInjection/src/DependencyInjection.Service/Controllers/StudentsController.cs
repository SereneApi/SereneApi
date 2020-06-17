using DependencyInjection.API.DTOs;
using DependencyInjection.Service.Mocking;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController: ControllerBase
    {
        [HttpGet("{studentId}")]
        public ActionResult<StudentDto> GetStudent(long studentId)
        {
            StudentDto student = StudentData.Students.FirstOrDefault(s => s.Id == studentId);

            if(student == null)
            {
                // It is important we return the correct response.
                // The ApiHandler uses this to confirm if the request was successful.
                // The message passed in this response will be returned in the ApiResponse.
                return NotFound($"Could not find a student with the Id:{studentId}");
            }

            return Ok(student);
        }

        [HttpGet]
        public ActionResult<List<StudentDto>> GetStudents()
        {
            if(StudentData.Students.Count <= 0)
            {
                return NoContent();
            }

            return Ok(StudentData.Students);
        }

        [HttpGet("SearchBy/GivenAndLastName")]
        public ActionResult<List<StudentDto>> FindByGivenAndLastName([FromQuery] StudentDto student)
        {
            List<StudentDto> students = StudentData.Students
                .Where(s => (string.IsNullOrWhiteSpace(student.GivenName) || s.GivenName.Equals(student.GivenName, StringComparison.InvariantCultureIgnoreCase)) && 
                            (string.IsNullOrWhiteSpace(student.LastName) || s.LastName.Equals(student.LastName, StringComparison.InvariantCultureIgnoreCase)))
                            .ToList();

            if(students.Count <= 0)
            {
                return NotFound("Could not find a student with the supplied Given and Last Name");
            }

            return Ok(students);
        }

        [HttpPost]
        public IActionResult Create([FromBody] StudentDto student)
        {
            if(student.Id != 0)
            {
                return BadRequest("The student cannot have an existing ID");
            }

            try
            {
                student.Id = StudentData.Students.Count;

                StudentData.Students.Add(student);
            }
            catch(Exception exception)
            {
                return Problem(exception.ToString());
            }

            return Ok();
        }

        [HttpGet("{studentId}/Classes")]
        public ActionResult<List<ClassDto>> GetStudentClasses(long studentId)
        {
            StudentDto student = StudentData.Students.FirstOrDefault(s => s.Id == studentId);

            if(student == null)
            {
                return NotFound($"Could not find a student with the Id:{studentId}");
            }

            return Ok(ClassDto.Classes);
        }
    }
}