﻿using Microsoft.AspNetCore.Mvc;
using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Test.TestControllers
{
    internal class PostsController : Controller
    {
        [HttpGet]
        public Post Get(int id)
        {
            return new Post();
        }
    }
}