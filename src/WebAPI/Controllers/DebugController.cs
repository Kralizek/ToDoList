using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DebugController : ControllerBase
    {
        public IEnumerable<string> Get([FromServices] IConfiguration configuration)
        {
            return configuration.AsEnumerable().Select(item => $"{item.Key}={item.Value}");
        }
    }
}