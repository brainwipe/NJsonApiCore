using Newtonsoft.Json;
using NJsonApi;
using NJsonApi.Infrastructure;
using NJsonApi.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace NJsonApiCore.Web.MVC5
{
    public class JsonApiActionFilter : IActionFilter
    {
        public bool AllowMultiple { get { return false; } }
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

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext context, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var contentType = context.Request.Content.Headers.ContentType;

            if (contentType == null || contentType.MediaType != configuration.DefaultJsonApiMediaType)
            {
                return new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }

            if (!ValidateAcceptHeader(context.Request.Headers))
            {
                return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
            }

            InternalActionExecuting(context, cancellationToken);

            HttpActionExecutedContext executedContext;

            if (context.Response != null)
            {
                return context.Response;
            }

            var response = await continuation();
            executedContext = new HttpActionExecutedContext(context, null)
            {
                Response = response
            };

            InternalActionExecuted(executedContext, cancellationToken);

            return context.Response;
        }

        public virtual void InternalActionExecuting(HttpActionContext context, CancellationToken cancellationToken)
        {
            if (context.ActionArguments.Any(a => a.Value is UpdateDocument))
            {
                var argument = context.ActionArguments.First(a => a.Value is UpdateDocument);
                var updateDocument = argument.Value as UpdateDocument;

                if (updateDocument != null)
                {
                    var typeInsideDeltaGeneric = GetTypeToUpdate(context);

                    var jsonApiContext = new Context(new Uri(context.Request.RequestUri.AbsoluteUri));
                    var transformed = jsonApiTransformer.TransformBack(updateDocument, typeInsideDeltaGeneric, jsonApiContext);
                    context.ActionArguments[argument.Key] = transformed;
                    context.ModelState.Clear();
                }
            }
        }

        private Type GetTypeToUpdate(HttpActionContext context)
        {
            var paramBinding = context.ActionDescriptor
                .ActionBinding
                .ParameterBindings
                .Single(x => x.WillReadBody);

            if (paramBinding.Descriptor.ParameterType.GetGenericTypeDefinition() != typeof(Delta<>))
            {
                throw new InvalidOperationException("The action parameter that represents the body of the request must be wrapped in the generic type Delta<YourEntity>");
            }

            return paramBinding.Descriptor.ParameterType.GetGenericArguments().First();
        }

        public virtual void InternalActionExecuted(HttpActionExecutedContext context, CancellationToken cancellationToken)
        {
            var content = context.Response.Content as ObjectContent;

            if (content == null)
            {
                return;
            }

            var relationshipPaths = FindRelationshipPathsToInclude(context.Request);

            if (!configuration.ValidateIncludedRelationshipPaths(relationshipPaths, content.Value))
            {
                // TODO - It would be better to give a little JSON Api styled error with details of the relationship
                // paths that were not found
                context.Response = context.Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (!context.Response.IsSuccessStatusCode)
            {
                // TODO - Deal with errors do GET first
                //var transformed = BadActionResultTransformer.Transform(context.Result);

                //context.Result = new ObjectResult(transformed)
                //{
                //    StatusCode = transformed.Errors.First().Status
                //};
                return;
            }

            var jsonApiContext = new Context(
                context.Request.RequestUri,
                relationshipPaths);
            var transformedIntoJsonApi = jsonApiTransformer.Transform(content.Value, jsonApiContext);

            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, transformedIntoJsonApi, configuration.DefaultJsonApiMediaType);
        }

        private string[] FindRelationshipPathsToInclude(HttpRequestMessage request)
        {
            var result = request.GetQueryNameValuePairs().Where(x => x.Key == "include").FirstOrDefault();

            return string.IsNullOrEmpty(result.Value) ? new string[0] : result.Value.Split(',');
        }

        // TODO - Merge into NJsonApiCore and remove from MVC support libraries
        private bool ValidateAcceptHeader(HttpRequestHeaders headers)
        {
            var acceptsHeaders = headers.Accept.FirstOrDefault().MediaType;

            if (string.IsNullOrEmpty(acceptsHeaders))
            {
                return true;
            }

            return acceptsHeaders
                .Split(',')
                .Select(x => x.Trim())
                .Any(x =>
                    x == "*/*" ||
                    x == configuration.DefaultJsonApiMediaType);
        }
    }
}