using NJsonApi.Serialization.Representations;

namespace NJsonApi.Web.MVC5.BadActionResultTransformers
{
    internal class TransformHttpNotFoundResult : BaseTransformBadAction<HttpNotFoundResult>
    {
        public override Error GetError(HttpNotFoundResult result)
        {
            return new Error()
            {
                Title = "The result was not found",
                Status = 404
            };
        }
    }
}