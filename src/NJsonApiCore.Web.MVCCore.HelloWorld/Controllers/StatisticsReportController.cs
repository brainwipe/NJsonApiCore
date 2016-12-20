using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NJsonApi.Web.MVCCore.HelloWorld.Models;
using NJsonApiCore.Web.MVCCore.HelloWorld.Models;

namespace NJsonApiCore.Web.MVCCore.HelloWorld.Controllers
{
    [Route("[controller]")]
    public class StatisticsReportController : Controller
    {
        [HttpGet]
        public IEnumerable<StatisticsReport> Get()
        {
            return new List<StatisticsReport>()
            {
                new StatisticsReport()
            };   
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new ObjectResult(new StatisticsReport());
        }
    }
    
}
