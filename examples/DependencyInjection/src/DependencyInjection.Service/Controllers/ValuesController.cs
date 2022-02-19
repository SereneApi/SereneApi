using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace DependencyInjection.Service.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("int/{value}")]
        public int GetInt(int value)
        {
            return value;
        }

        [HttpGet("GetSamplePdf")]
        public IActionResult GetSamplePdf()
        {
            string directory = Directory.GetCurrentDirectory();
            string fileName = "sample.pdf";

            string filePath = $"{directory}\\{fileName}";

            FileStream file = new FileStream(filePath, FileMode.Open);

            return File(file, "application/pdf");
        }

        [HttpGet("string/{value}")]
        public string GetString(string value)
        {
            return value;
        }
    }
}