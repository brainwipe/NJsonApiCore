using Microsoft.AspNetCore.Mvc;
using NJsonApi.Serialization.Documents;

namespace NJsonApi.Web.MVCCore.BadActionResultTransformers
{
    internal interface ICanTransformBadActions
    {
        bool Accepts(IActionResult result);

        CompoundDocument Transform(IActionResult result);
    }
}