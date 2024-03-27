using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Web.Http.Filters;

namespace WebAPI.Filters
{
    public class GlobalHttpHeaderFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter { Name = "x-user", In = ParameterLocation.Header, Description = "用户ID", Required = false });
            operation.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter { Name = "x-corp", In = ParameterLocation.Header, Description = "公司", Required = false });
            operation.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter { Name = "x-domain", In = ParameterLocation.Header, Description = "域", Required = false });
            operation.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter { Name = "x-language", In = ParameterLocation.Header, Description = "语言", Required = false });

        }
    }
}
