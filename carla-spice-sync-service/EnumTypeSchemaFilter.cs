using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class EnumTypeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            

            if (context.GetType().IsEnum)
            {
                var obj = new OpenApiObject();
                obj["name"] = new OpenApiString(context.GetType().Name);
                obj["modelAsString"] = new OpenApiBoolean(false);
                schema.Extensions.Add(
                    "x-ms-enum",
                    obj);
            }
        }

    }
}
