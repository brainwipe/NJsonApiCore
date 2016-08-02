using Newtonsoft.Json;
using NJsonApi.Serialization;
using NJsonApi.Web.MVCCore.BadActionResultTransformers;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NJsonApi.Web
{
    public class JsonApiActionFilter : ActionFilterAttribute
    {
        public bool AllowMultiple => false;
        private readonly IJsonApiTransformer jsonApiTransformer;
        private readonly IConfiguration configuration;
        private readonly JsonSerializer serializer;

        public JsonApiActionFilter(
            IJsonApiTransformer jsonApiTransformer,
            IConfiguration configuration,
            JsonSerializer serializer)
        {
            this.jsonApiTransformer = jsonApiTransformer;
            this.configuration = configuration;
            this.serializer = serializer;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.ContentType != configuration.DefaultJsonApiMediaType)
            {
                context.Result = new UnsupportedMediaTypeResult();
            }

            if (!ValidateAcceptHeader(context.HttpContext.Request.Headers))
            {
                context.Result = new StatusCodeResult(406);
            }

            using (var reader = new StreamReader(context.HttpContext.Request.Body))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var updateDocument = serializer.Deserialize(jsonReader, typeof(UpdateDocument)) as UpdateDocument;

                    if (updateDocument != null)
                    {
                        var actionDescriptorForBody = context.ActionDescriptor
                            .Parameters
                            .Single(x => x.BindingInfo.BindingSource == BindingSource.Body);

                        var typeInsideDeltaGeneric = actionDescriptorForBody
                            .ParameterType
                            .GenericTypeArguments
                            .Single();
                        var jsonApiContext = new Context(new Uri(context.HttpContext.Request.Host.Value, UriKind.Absolute));
                        var transformed = jsonApiTransformer.TransformBack(updateDocument, typeInsideDeltaGeneric, jsonApiContext);
                        context.ActionArguments.Add(actionDescriptorForBody.Name, transformed);
                        context.ModelState.Clear();
                    }
                }
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result == null || context.Result is NoContentResult)
            {
                return;
            }

            if (BadActionResultTransformer.IsBadAction(context.Result))
            {
                var transformed = BadActionResultTransformer.Transform(context.Result);

                context.Result = new ObjectResult(transformed)
                {
                    StatusCode = transformed.Errors.First().Status
                };
                return;
            }

            var responseResult = (ObjectResult)context.Result;
            var relationshipPaths = FindRelationshipPathsToInclude(context.HttpContext.Request);

            if (!configuration.ValidateIncludedRelationshipPaths(relationshipPaths, responseResult.Value))
            {
                context.Result = new StatusCodeResult(400);
                return;
            }

            var jsonApiContext = new Context(
                new Uri(context.HttpContext.Request.GetDisplayUrl()),
                relationshipPaths);
            responseResult.Value = jsonApiTransformer.Transform(responseResult.Value, jsonApiContext);
        }

        private string[] FindRelationshipPathsToInclude(HttpRequest request)
        {
            var includeQueryParameter = request.Query["include"].FirstOrDefault();

            return configuration.FindRelationshipPathsToInclude(includeQueryParameter);
        }

        private bool ValidateAcceptHeader(IHeaderDictionary headers)
        {
            return configuration.ValidateAcceptHeader(headers["Accept"].FirstOrDefault());
        }
    }
}