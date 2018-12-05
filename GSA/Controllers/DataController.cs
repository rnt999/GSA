using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GSA.Controllers
{
    [Route("api")]
    public class DataController : Controller
    {
        [HttpGet("monthly-capital")]
        public IActionResult GetMonthlyCapital([FromQuery] string[] strategies)
        {
            return Ok("");
        }
    }
}
