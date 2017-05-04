using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NJsonApi.Test.TestModel
{
    internal class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dimensions Dimensions { get; set; }
    }
}
