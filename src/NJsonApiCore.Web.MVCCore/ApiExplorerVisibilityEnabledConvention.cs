using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace NJsonApi.Web.MVCCore
{
    public class ApiExplorerVisibilityEnabledConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (controller.ApiExplorer.IsVisible == null)
                {
                    controller.ApiExplorer.IsVisible = true;
                    controller.ApiExplorer.GroupName = controller.ControllerName;
                }
            }
        }
    }
}