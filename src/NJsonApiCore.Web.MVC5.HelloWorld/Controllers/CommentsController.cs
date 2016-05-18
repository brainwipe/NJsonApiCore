using NJsonApi.Web.MVC5.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NJsonApi.Web.MVC6.HelloWorld.Controllers
{
    [Route("comments")]
    public class CommentsController : ApiController
    {
        [HttpGet]
        public IEnumerable<Comment> Get()
        {
            return StaticPersistentStore.Comments;
        }

        [Route("{id}")]
        [HttpGet]
        public Comment Get(int id)
        {
            return StaticPersistentStore.Comments.Single(w => w.Id == id);
        }
    }
}