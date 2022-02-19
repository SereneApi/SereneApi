using DependencyInjection.API;
using Microsoft.AspNetCore.Mvc;
using SereneApi.Core.Http.Responses;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DependencyInjection.WebUi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IValuesApi _valuesApi;

        public ValuesController(IValuesApi valuesApi)
        {
            _valuesApi = valuesApi;
        }

        [HttpGet("int/{value}")]
        public async Task<ActionResult<IApiResponse<int>>> GetIntAsync(int value)
        {
            IApiResponse<int> response = await _valuesApi.GetAsync(value);

            return Ok(response);
        }

        [HttpGet("GetSamplePdf")]
        public async Task<IActionResult> GetSamplePdfAsync()
        {
            Guid fileGuid = Guid.NewGuid();

            string directory = Directory.GetCurrentDirectory();
            string fileName = $"{fileGuid}.pdf";

            string filePath = $"{directory}\\{fileName}";

            IApiResponse<MemoryStream> response = await _valuesApi.GetSamplePfgAsync();

            await using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                response.Data.Seek(0, SeekOrigin.Begin);

                await response.Data.CopyToAsync(fileStream);
            }

            return Ok(filePath);
        }

        [HttpGet("string/{value}")]
        public async Task<ActionResult<IApiResponse<string>>> GetStringAsync(string value)
        {
            IApiResponse<string> response = await _valuesApi.GetAsync(value);

            return Ok(response);
        }
    }
}