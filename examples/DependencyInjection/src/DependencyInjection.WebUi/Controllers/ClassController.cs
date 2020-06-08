using DependencyInjection.API;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInjection.WebUi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassApi _classApi;

        public ClassController(IClassApi classApi)
        {
            _classApi = classApi;
        }
    }
}