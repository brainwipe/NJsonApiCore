using Microsoft.AspNetCore.Mvc;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Web.MVCCore.BadActionResultTransformers
{
    internal class TransformBadRequestResult : BaseTransformBadAction<BadRequestResult>
    {
        public override Error GetError(BadRequestResult result)
        {
            return new Error()
            {
                Title = $"There was a bad request.",
                Status = 400
            };
        }
    }
}