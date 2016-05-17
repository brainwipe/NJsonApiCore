using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: OwinStartup(typeof(NJsonApiCore.Web.MVC5.HelloWorld.Startup))]

namespace NJsonApiCore.Web.MVC5.HelloWorld
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}