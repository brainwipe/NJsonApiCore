using Microsoft.AspNetCore.Mvc;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Web.MVCCore.BadActionResultTransformers
{
    internal class TransformHttpNotFoundResult : BaseTransformBadAction<NotFoundResult>
    {
        public override Error GetError(NotFoundResult result)
        {
            return new Error()
            {
                Title = "The result was not found",
                Status = 404
            };
        }
    }
}