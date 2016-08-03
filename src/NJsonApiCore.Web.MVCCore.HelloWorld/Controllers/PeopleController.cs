using NJsonApi.Web.MVCCore.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace NJsonApi.Web.MVCCore.HelloWorld.Controllers
{
    [Route("[controller]")]
    public class PeopleController : Controller
    {
        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return StaticPersistentStore.People;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new ObjectResult(StaticPersistentStore.People.Single(w => w.Id == id));
        }
    }
}