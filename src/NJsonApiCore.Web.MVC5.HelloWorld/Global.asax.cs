using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NJsonApiCore.Web.MVC5.HelloWorld
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new UnityContainer();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            UnityConfig.RegisterComponents(container, GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(container, GlobalConfiguration.Configuration.Filters);
        }
    }
}