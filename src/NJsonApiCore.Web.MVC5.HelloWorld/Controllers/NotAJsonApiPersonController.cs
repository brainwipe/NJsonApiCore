using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using NJsonApi.Web.MVC5.HelloWorld.Models;

namespace NJsonApiCore.Web.MVC5.HelloWorld.Controllers
{
    [RoutePrefix("notajsonapiperson")]
    public class NotAJsonApiPersonController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return StaticPersistentStore.People;
        }
    }
}