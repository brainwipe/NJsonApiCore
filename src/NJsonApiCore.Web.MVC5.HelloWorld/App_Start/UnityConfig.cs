using Microsoft.Practices.Unity;
using NJsonApi.Serialization;
using NJsonApi.Web.MVC5.Serialization;
using System.Web.Http;
using System.Web.Mvc;
using Unity.Mvc5;

namespace NJsonApiCore.Web.MVC5.HelloWorld
{
    public static class UnityConfig
    {
        public static void RegisterComponents(IUnityContainer container, HttpConfiguration configuration)
        {
            var nJsonApiConfig = NJsonApiConfiguration.BuildConfiguration();

            container.RegisterInstance<ILinkBuilder>(new LinkBuilder(configuration));
            container.RegisterInstance(nJsonApiConfig.GetJsonSerializer());
            container.RegisterType<IJsonApiTransformer, JsonApiTransformer>();
            container.RegisterInstance(nJsonApiConfig);
            container.RegisterType<TransformationHelper>();
            container.RegisterType<JsonApiActionFilter>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}