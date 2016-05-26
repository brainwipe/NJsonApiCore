using Microsoft.Practices.Unity;
using NJsonApi;
using NJsonApi.Serialization;
using NJsonApi.Web.MVC5.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace NJsonApiCore.Web.MVC5
{
    public static class JsonApiAppStart
    {
        public static void Configure(
            HttpConfiguration dotNetConfiguration,
            IUnityContainer container,
            IConfiguration jsonApiConfiguration)
        {
            container.RegisterInstance(dotNetConfiguration);
            container.RegisterInstance(jsonApiConfiguration.GetJsonSerializer());
            container.RegisterType<IJsonApiTransformer, JsonApiTransformer>();
            container.RegisterInstance(jsonApiConfiguration);
            container.RegisterType<TransformationHelper>();
            container.RegisterType<JsonApiActionFilter>();
            container.RegisterType<IApiExplorer, ApiExplorer>();
            container.RegisterType<ILinkBuilder, LinkBuilder>();
            container.RegisterType<JsonApiMediaTypeFormatter>();

            dotNetConfiguration.Formatters.Add(container.Resolve<JsonApiMediaTypeFormatter>());
            dotNetConfiguration.Filters.Add(container.Resolve<JsonApiActionFilter>());
        }
    }
}