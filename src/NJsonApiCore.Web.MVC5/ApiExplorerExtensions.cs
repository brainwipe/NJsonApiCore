using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;

namespace NJsonApiCore.Web.MVC5
{
    internal static class ApiExplorerExtensions
    {
        public static IEnumerable<ApiDescription> From(this IApiExplorer provider, Type controller)
        {
            return provider.ApiDescriptions.Where(a =>
                a.ActionDescriptor.ControllerDescriptor.ControllerType.FullName == controller.FullName);
        }
    }
}