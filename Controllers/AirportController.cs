using Microsoft.AspNetCore.Mvc;

namespace Airport.Controllers
{
    [ApiController]
    [Route("airport")]
    public class AirportController : ControllerBase
    {
        [HttpPost("create")]
        public IActionResult Create([FromBody] object airportData)
        {

            return Ok(new { message = "Aeroporto criado com sucesso." });
        }
    }
}
