using Microsoft.AspNet.Mvc;
using NJsonApi.Web.MVC6.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Web.MVC6.HelloWorld.Controllers
{
    [Route("[controller]")]
    public class TestExamplesController : Controller
    {
        [HttpGet]
        [Route("[action]")]
        public void ThrowException()
        {
            throw new NotImplementedException("An example exception thrown");
        }

        [HttpGet]
        [Route("[action]")]
        public IEnumerable<Article> GetEmptyList()
        {
            return new List<Article>();
        }
    }
}