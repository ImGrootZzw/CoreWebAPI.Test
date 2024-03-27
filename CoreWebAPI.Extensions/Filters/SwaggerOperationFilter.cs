using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using CoreWebAPI.Common.Helper;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using CoreWebAPI.Common.LogHelper;

namespace CoreWebAPI.Filters
{
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var apiExplorerSettings = controllerActionDescriptor
                    .ControllerTypeInfo
                    .GetCustomAttributes(typeof(SwaggerTagAttribute), true)
                    .Cast<SwaggerTagAttribute>()
                    .FirstOrDefault();
                if (controllerActionDescriptor.ControllerName.ToLower().Contains("produce"))
                {
                    var s = controllerActionDescriptor.ControllerName;
                }
                    
                if (apiExplorerSettings != null && !string.IsNullOrWhiteSpace(apiExplorerSettings.Description))
                {
                    operation.Tags = new List<OpenApiTag> { new OpenApiTag { Name = apiExplorerSettings.Description } };
                }
                else
                {
                    operation.Tags = new List<OpenApiTag>
                    {new OpenApiTag {Name = controllerActionDescriptor.ControllerName}};
                }
            }
        }
    }

}

