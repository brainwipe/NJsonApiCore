using NJsonApi.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace NJsonApi.Web.MVCCore.BadActionResultTransformers
{
    public static class BadActionResultTransformer
    {
        private static readonly List<ICanTransformBadActions> badActionRegistry = new List<ICanTransformBadActions>();

        static BadActionResultTransformer()
        {
            badActionRegistry.Add(new TransformBadRequestObjectResult());
            badActionRegistry.Add(new TransformBadRequestResult());
            badActionRegistry.Add(new TransformHttpNotFoundObjectResult());
            badActionRegistry.Add(new TransformHttpNotFoundResult());
            badActionRegistry.Add(new TransformHttpUnauthorizedResult());
        }

        private static ICanTransformBadActions FindTransformer(IActionResult badActionResult)
        {
            return badActionRegistry.Single(x => x.Accepts(badActionResult));
        }

        public static bool IsBadAction(IActionResult potentiallyBadsAction)
        {
            return badActionRegistry.Any(x => x.Accepts(potentiallyBadsAction));
        }

        public static CompoundDocument Transform(IActionResult badActionResult)
        {
            return FindTransformer(badActionResult).Transform(badActionResult);
        }
    }
}