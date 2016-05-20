using Microsoft.Practices.Unity;
using System.Web.Http.Filters;

namespace NJsonApiCore.Web.MVC5.HelloWorld
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(IUnityContainer container, HttpFilterCollection filters)
        {
            var jsonApiActionFilter = container.Resolve<JsonApiActionFilter>();

            filters.Add(jsonApiActionFilter);
        }
    }
}