using Microsoft.AspNetCore.Mvc;
using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Test.TestControllers
{
    internal class ControllerWithMoreThanOneGetForAResource : Controller
    {
        [HttpGet]
        public Post First(int id)
        {
            return new Post();
        }

        [HttpGet]
        public Post Second(int id)
        {
            return new Post();
        }
    }
}