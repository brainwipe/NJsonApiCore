using NJsonApi.Web.MVC5.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NJsonApi.Web.MVC5.HelloWorld.Controllers
{
    [RoutePrefix("testexamples")]
    public class TestExamplesController : ApiController
    {
        [HttpGet]
        [Route("throwexception")]
        public void ThrowException()
        {
            throw new NotImplementedException("An example exception thrown");
        }

        [HttpGet]
        [Route("getemptylist")]
        public IEnumerable<Article> GetEmptyList()
        {
            return new List<Article>();
        }
    }
}