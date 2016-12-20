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
    public class ReportsController
    {
        [HttpGet]
        public IEnumerable<Report> Get()
        {
            return new List<Report>() { new Report() };
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (id != 1)
            {
                return new NotFoundResult();
            }

            return new ObjectResult(new Report());
        }

        [HttpGet("{reportId}/statisticsReport")]
        public IActionResult GetStatisticsReport(int reportId)
        {
            return new ObjectResult(new Report().StatisticsReport);
        }
    }
}
