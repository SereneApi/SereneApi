using DependencyInjection.API.DTOs;
using DependencyInjection.Service.Mocking;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DependencyInjection.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<StudentDto> GetStudent(long id)
        {
            StudentDto student = StudentData.Students.FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                // It is important we return the correct response.
                // The ApiHandler uses this to confirm if the request was successful.
                // The message passed in this response will be returned in the ApiResponse.
                return NotFound($"Could not find a student with the Id:{id}");
            }

            return Ok(student);
        }
    }
}