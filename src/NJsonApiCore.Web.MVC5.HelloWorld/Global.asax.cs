using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Unity.Mvc5;

namespace NJsonApiCore.Web.MVC5.HelloWorld
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new UnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var jsonApiConfiguration = MyNJsonApiConfigurationBuilder.BuildConfiguration();
            JsonApiAppStart.Configure(GlobalConfiguration.Configuration, container, jsonApiConfiguration);
        }
    }
}