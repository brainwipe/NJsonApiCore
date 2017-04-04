using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Test.TestModel
{
    public class SimpleTypes
    {
        public int TestInt { get; set; }
        public int? NullableInt { get; set; }

        public double TestDouble { get; set; }
        public double? NullableDouble { get; set; }
    }
}
