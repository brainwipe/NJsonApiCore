using Microsoft.AspNetCore.Mvc;
using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Test.TestControllers
{
    internal class ModelThatCausesInfiniteLoopController : Controller
    {
        [HttpGet]
        public ModelThatCausesInfiniteLoop Get(int id)
        {
            return new ModelThatCausesInfiniteLoop();
        }
    }
}