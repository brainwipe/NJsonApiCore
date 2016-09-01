using Microsoft.AspNetCore.Mvc;
using NJsonApi.Web.MVCCore.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Web.MVCCore.HelloWorld.Controllers.Tests
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