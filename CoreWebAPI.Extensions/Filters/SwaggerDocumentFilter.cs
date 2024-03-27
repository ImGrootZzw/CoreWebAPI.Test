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

namespace CoreWebAPI.Filters
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var tags = new List<OpenApiTag>
            {
                new OpenApiTag {
                    Name = "PiproduceMstr",
                    Description = "Authentication",
                    ExternalDocs = new OpenApiExternalDocs { Description = "Authentication" }
                },
                new OpenApiTag {
                    Name = "Localization",
                    Description = "Localization",
                    ExternalDocs = new OpenApiExternalDocs { Description = "Localization" }
                }
            };

            var sourceTages = swaggerDoc.Tags;
            foreach(var tag in sourceTages)
            {
                tag.Name = tag.Description;
            }

            swaggerDoc.Tags = tags.OrderBy(x => x.Name).ToList();

            var apis = context.ApiDescriptions.Where(x => x.RelativePath.Contains("abp"));
            if (apis.Any())
            {
                foreach (var item in apis)
                {
                    swaggerDoc.Paths.Remove("/" + item.RelativePath);
                }
            }
        }
    }

}

