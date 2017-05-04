using Microsoft.AspNetCore.Mvc;
using NJsonApiCore.Test.TestModel;

namespace NJsonApi.Test.TestControllers
{
    internal class WidgetsController
    {
        [HttpGet]
        public Widget Get(int id)
        {
            return new Widget();
        }
    }
}
