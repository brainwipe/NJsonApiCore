using NJsonApiCore.Web.MVC5.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NJsonApiCore.Web.MVC5.HelloWorld.Controllers
{
    [RoutePrefix("simplestpossibles")]
    public class SimplestPossibleController : ApiController
    {
        [Route("{id}")]
        [HttpGet]
        public SimplestPossibleModel Get(int id)
        {
            return new SimplestPossibleModel() { Id = 1 };
        }
    }
}