using Microsoft.AspNetCore.Mvc;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Web.MVCCore.BadActionResultTransformers
{
    internal class TransformHttpUnauthorizedResult : BaseTransformBadAction<UnauthorizedResult>
    {
        public override Error GetError(UnauthorizedResult result)
        {
            return new Error()
            {
                Title = "You were not authorised.",
                Status = 403
            };
        }
    }
}