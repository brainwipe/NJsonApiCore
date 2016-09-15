using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NJsonApi.Serialization;
using NJsonApi.Web.MVCCore.BadActionResultTransformers;
using System;
using System.Linq;
using NJsonApi.Infrastructure;

namespace NJsonApi.Web
{
    public class JsonApiActionFilter : ActionFilterAttribute
    {
        public bool AllowMultiple => false;
        private readonly IJsonApiTransformer jsonApiTransformer;
        private readonly IConfiguration configuration;

        public JsonApiActionFilter(
            IJsonApiTransformer jsonApiTransformer,
            IConfiguration configuration)
        {
            this.jsonApiTransformer = jsonApiTransformer;
            this.configuration = configuration;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isValidContentType = context.HttpContext.Request.ContentType == configuration.DefaultJsonApiMediaType;
            var controllerType = context.Controller.GetType();
            var isControllerRegistered = configuration.IsControllerRegistered(controllerType);

            if (isControllerRegistered)
            {
                if (!isValidContentType && context.HttpContext.Request.Method != "GET")
                {
                    context.Result = new UnsupportedMediaTypeResult();
                    return;
                }

                if (!ValidateAcceptHeader(context.HttpContext.Request.Headers))
                {
                    context.Result = new StatusCodeResult(406);
                    return;
                }

                var actionDescriptorForBody = context.ActionDescriptor
                    .Parameters
                    .Single(x => x.BindingInfo != null && x.BindingInfo.BindingSource == BindingSource.Body);

                if (context.ActionArguments.ContainsKey(actionDescriptorForBody.Name))
                {
                    context.ActionArguments[actionDescriptorForBody.Name] = context.ActionDescriptor.Properties[actionDescriptorForBody.Name] as IDelta;
                }
                else
                {
                    context.ActionArguments.Add(actionDescriptorForBody.Name, context.ActionDescriptor.Properties[actionDescriptorForBody.Name] as IDelta);
                }

                context.ModelState.Clear();
            }
            else
            {
                if (isValidContentType)
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = 406,
                        Content = $"The Content-Type provided was {context.HttpContext.Request.ContentType} but there was no NJsonApiCore configuration mapping for {controllerType.FullName}"
                    };
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

            var controllerType = context.Controller.GetType();
            var isControllerRegistered = configuration.IsControllerRegistered(controllerType);

            if (isControllerRegistered)
            {
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