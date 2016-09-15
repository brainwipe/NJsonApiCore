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
            var actionDescriptorForBody = context.ActionDescriptor.Parameters.SingleOrDefault(
                    x => x.BindingInfo != null && x.BindingInfo.BindingSource == BindingSource.Body);
            if (actionDescriptorForBody == null)
            {
                return;
            }

            using (var reader = new StreamReader(context.HttpContext.Request.Body))
            {
                if (context.ActionDescriptor.Properties.ContainsKey(actionDescriptorForBody.Name))
                {
                    context.ActionDescriptor.Properties[actionDescriptorForBody.Name] = reader.ReadToEnd();
                }
                else
                {
                    context.ActionDescriptor.Properties.Add(actionDescriptorForBody.Name, reader.ReadToEnd());
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