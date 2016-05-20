using NJsonApi.Serialization.Representations;

namespace NJsonApi.Web.MVC5.BadActionResultTransformers
{
    internal class TransformHttpUnauthorizedResult : BaseTransformBadAction<HttpUnauthorizedResult>
    {
        public override Error GetError(HttpUnauthorizedResult result)
        {
            return new Error()
            {
                Title = "You were not authorised.",
                Status = 403
            };
        }
    }
}