using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Test.TestModel
{
    public class ModelThatCausesInfiniteLoop
    {
        public int Id { get; set; }

        public ModelThatCausesInfiniteLoop Child { get; set; }
    }
}