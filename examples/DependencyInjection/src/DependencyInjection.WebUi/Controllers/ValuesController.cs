using DependencyInjection.API;
using Microsoft.AspNetCore.Mvc;
using SereneApi;
using System.Threading.Tasks;

namespace DependencyInjection.WebUi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController: ControllerBase
    {
        private readonly IValuesApi _valuesApi;

        public ValuesController(IValuesApi valuesApi)
        {
            _valuesApi = valuesApi;
        }

        [HttpGet("int/{value}")]
        public async Task<ActionResult<IApiResponse<int>>> GetIntAsync(int value)
        {
            IApiResponse<int> response = await _valuesApi.GetIntAsync(value);

            return Ok(response);
        }
    }
}
