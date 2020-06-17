using DependencyInjection.API;
using DependencyInjection.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using SereneApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DependencyInjection.WebUi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController: ControllerBase
    {
        private readonly IStudentApi _studentApi;

        public StudentsController(IStudentApi studentApi)
        {
            _studentApi = studentApi;
        }

        [HttpGet("{studentId}")]
        public async Task<ActionResult<IApiResponse<StudentDto>>> GetStudent(long studentId)
        {
            IApiResponse<StudentDto> response = await _studentApi.GetAsync(studentId);

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentDto>>> GetStudents()
        {
            IApiResponse<List<StudentDto>> response = await _studentApi.GetAllAsync();

            return Ok(response);
        }

        [HttpGet("GivenAndLastName")]
        public async Task<ActionResult<List<StudentDto>>> FindByGivenAndLastName([FromQuery] StudentDto student)
        {
            IApiResponse<List<StudentDto>> response = await _studentApi.FindByGivenAndLastName(student);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentDto student)
        {
            IApiResponse response = await _studentApi.CreateAsync(student);

            return Ok(response);
        }

        [HttpGet("{studentId}/Classes")]
        public async Task<ActionResult<List<ClassDto>>> GetStudentClasses(int studentId)
        {
            IApiResponse<List<ClassDto>> response = await _studentApi.GetStudentClassesAsync(studentId);

            return Ok(response);
        }
    }
}