using Microsoft.AspNetCore.Mvc;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Web.MVCCore.BadActionResultTransformers
{
    internal class TransformBadRequestObjectResult : BaseTransformBadAction<BadRequestObjectResult>
    {
        public override Error GetError(BadRequestObjectResult result)
        {
            return new Error()
            {
                Title = $"There was a bad request for {result.Value}",
                Status = result.StatusCode.Value
            };
        }
    }
}