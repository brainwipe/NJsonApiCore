using NJsonApi.Web.MVC5.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NJsonApi.Web.MVC6.HelloWorld.Controllers
{
    [Route("People")]
    public class PeopleController : ApiController
    {
        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return StaticPersistentStore.People;
        }

        [Route("{id}")]
        [HttpGet]
        public Person Get(int id)
        {
            return StaticPersistentStore.People.Single(w => w.Id == id);
        }
    }
}