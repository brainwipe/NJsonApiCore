using Microsoft.AspNet.Mvc;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Web.MVC6.BadActionResultTransformers
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