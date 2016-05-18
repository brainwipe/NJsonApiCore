using NJsonApi.Web.MVC5.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NJsonApi.Web.MVC5.HelloWorld.Controllers
{
    [RoutePrefix("people")]
    public class PeopleController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return StaticPersistentStore.People;
        }

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            return Ok(StaticPersistentStore.People.Single(w => w.Id == id));
        }
    }
}