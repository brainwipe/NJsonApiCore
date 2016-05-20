using NJsonApi.Serialization.Representations;

namespace NJsonApi.Web.MVC5.BadActionResultTransformers
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