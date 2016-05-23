using Microsoft.Practices.Unity;
using NJsonApi.Serialization;
using NJsonApi.Web.MVC5.Serialization;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Unity.Mvc5;

namespace NJsonApiCore.Web.MVC5.HelloWorld
{
    public static class UnityConfig
    {
        public static void RegisterComponents(IUnityContainer container, HttpConfiguration configuration)
        {
            var nJsonApiConfig = NJsonApiConfiguration.BuildConfiguration();

            container.RegisterInstance(configuration);
            container.RegisterInstance(nJsonApiConfig.GetJsonSerializer());
            container.RegisterType<IJsonApiTransformer, JsonApiTransformer>();
            container.RegisterInstance(nJsonApiConfig);
            container.RegisterType<TransformationHelper>();
            container.RegisterType<JsonApiActionFilter>();
            container.RegisterType<IApiExplorer, ApiExplorer>();
            container.RegisterType<ILinkBuilder, LinkBuilder>();
            container.RegisterType<JsonApiMediaTypeFormatter>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}