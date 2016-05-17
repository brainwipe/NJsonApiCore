using System.Web;
using System.Web.Mvc;

namespace NJsonApiCore.Web.MVC5.HelloWorld
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
