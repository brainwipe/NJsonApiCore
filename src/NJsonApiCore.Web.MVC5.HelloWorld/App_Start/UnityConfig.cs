using Microsoft.Practices.Unity;
using NJsonApi;
using NJsonApi.Serialization;
using NJsonApi.Web.MVC5.Serialization;
using System.Web.Http;
using System.Web.Mvc;
using Unity.Mvc5;

namespace NJsonApiCore.Web.MVC5.HelloWorld
{
    public static class UnityConfig
    {
        public static void RegisterComponents(
            HttpConfiguration configuration,
            IConfiguration nJsonApiConfig)
        {
            var container = new UnityContainer();

            container.RegisterInstance<ILinkBuilder>(new LinkBuilder(configuration));
            container.RegisterInstance(nJsonApiConfig.GetJsonSerializer());
            container.RegisterType<IJsonApiTransformer, JsonApiTransformer>();
            container.RegisterInstance(nJsonApiConfig);
            container.RegisterType<TransformationHelper>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}