using Microsoft.AspNet.Mvc;
using NJsonApi.Serialization.Documents;

namespace NJsonApi.Web.MVC6.BadActionResultTransformers
{
    internal interface ICanTransformBadActions
    {
        bool Accepts(IActionResult result);

        CompoundDocument Transform(IActionResult result);
    }
}