using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class AutoRestParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var type = context.ApiParameterDescription.Type;

            if (type.IsEnum)
            {
                // parameter.Extensions.Add( "x-ms-enum", new { name = type.Name, modelAsString = false });
                var obj = new OpenApiObject
                {
                    ["name"] = new OpenApiString(type.Name),
                    ["modelAsString"] = new OpenApiBoolean(false)
                };
                parameter.Extensions.Add("x-ms-enum", obj);
            }
        }
    }
}
