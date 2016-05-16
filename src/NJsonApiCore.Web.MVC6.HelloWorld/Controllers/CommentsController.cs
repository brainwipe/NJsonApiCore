using Microsoft.AspNet.Mvc;
using NJsonApi.Web.MVC6.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Web.MVC6.HelloWorld.Controllers
{
    [Route("[controller]")]
    public class CommentsController : Controller
    {
        [HttpGet]
        public IEnumerable<Comment> Get()
        {
            return StaticPersistentStore.Comments;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new ObjectResult(StaticPersistentStore.Comments.Single(w => w.Id == id));
        }
    }
}