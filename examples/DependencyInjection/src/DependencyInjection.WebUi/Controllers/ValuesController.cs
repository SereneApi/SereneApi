using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjection.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SereneApi;
using SereneApi.Interfaces.Requests;

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
