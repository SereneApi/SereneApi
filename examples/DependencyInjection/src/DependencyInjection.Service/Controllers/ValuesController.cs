using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInjection.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController: ControllerBase
    {
        [HttpGet("int/{value}")]
        public int GetInt(int value)
        {
            return value;
        }

        [HttpGet("string/{value}")]
        public async Task<string> GetString(string value)
        {
            return value;
        }
    }
}
