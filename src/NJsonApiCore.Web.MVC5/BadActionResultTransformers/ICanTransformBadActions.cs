using NJsonApi.Serialization.Documents;

namespace NJsonApi.Web.MVC5.BadActionResultTransformers
{
    internal interface ICanTransformBadActions
    {
        bool Accepts(IActionResult result);

        CompoundDocument Transform(IActionResult result);
    }
}