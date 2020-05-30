using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInjection.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public Task<IActionResult> GetUsers(long id)
        {

        }
    }
}