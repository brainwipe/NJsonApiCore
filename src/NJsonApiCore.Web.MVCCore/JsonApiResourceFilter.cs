using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using NJsonApi.Serialization;
using NJsonApi.Web.MVCCore.BadActionResultTransformers;
using System;
using System.IO;
using System.Linq;

namespace NJsonApi.Web
{
    public class JsonApiResourceFilter : Attribute, IResourceFilter
    {
        private readonly IJsonApiTransformer jsonApiTransformer;
        private readonly JsonSerializer serializer;

        public JsonApiResourceFilter(
            IJsonApiTransformer jsonApiTransformer,
            JsonSerializer serializer)
        {
            this.jsonApiTransformer = jsonApiTransformer;
            this.serializer = serializer;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.ActionDescriptor.Parameters.Any(
                x => x.BindingInfo != null && x.BindingInfo.BindingSource == BindingSource.Body))
            {
                using (var reader = new StreamReader(context.HttpContext.Request.Body))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        var updateDocument = serializer.Deserialize(jsonReader, typeof(UpdateDocument)) as UpdateDocument;

                        if (updateDocument != null)
                        {
                            var actionDescriptorForBody = context.ActionDescriptor
                                .Parameters
                                .Single(x => x.BindingInfo != null && x.BindingInfo.BindingSource == BindingSource.Body);

                            var typeInsideDeltaGeneric = actionDescriptorForBody
                                .ParameterType
                                .GenericTypeArguments
                                .Single();

                            var jsonApiContext = new Context(new Uri(context.HttpContext.Request.Host.Value, UriKind.Absolute));
                            var transformed = jsonApiTransformer.TransformBack(updateDocument, typeInsideDeltaGeneric, jsonApiContext);

                            if (context.ActionDescriptor.Properties.ContainsKey(actionDescriptorForBody.Name))
                            {
                                context.ActionDescriptor.Properties[actionDescriptorForBody.Name] = transformed;
                            }
                            else
                            {
                                context.ActionDescriptor.Properties.Add(actionDescriptorForBody.Name, transformed);
                            }
                        }
                    }
                }
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
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
            }
        }
    }
}