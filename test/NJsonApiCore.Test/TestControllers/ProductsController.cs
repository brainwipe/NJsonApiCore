using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using NJsonApi.Test.TestModel;

namespace NJsonApi.Test.TestControllers
{
    internal class ProductsController : Controller
    {
        [HttpGet]
        public Product Get(int id)
        {
            return new Product();
        }
    }
}
