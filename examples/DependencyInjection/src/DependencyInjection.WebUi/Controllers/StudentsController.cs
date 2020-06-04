using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiCommon;
using DependencyInjection.API;
using DependencyInjection.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInjection.WebUi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentApi _studentApi;

        public StudentsController(IStudentApi studentApi)
        {
            _studentApi = studentApi;
        }

        [HttpGet("{studentId}")]
        public async Task<ActionResult<IApiResponse<StudentDto>>> GetStudent(long studentId)
        {
            var response = await _studentApi.GetAsync(studentId);

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentDto>>> GetStudents()
        {
            var response = await _studentApi.GetAllAsync();

            return Ok(response);
        }

        //public ActionResult<List<StudentDto>> FindByGivenAndLastName([FromQuery] StudentDto student)
        //{

        //}

        //public IActionResult Create([FromQuery] StudentDto student)
        //{

        //}

        //public ActionResult<List<ClassDto>> GetStudentClasses([FromQuery] StudentDto student)
        //{

        //}
    }
}