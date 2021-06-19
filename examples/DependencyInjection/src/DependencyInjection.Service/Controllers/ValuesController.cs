using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("string/{value}")]
        public string GetString(string value)
        {
            return value;
        }
    }
}
