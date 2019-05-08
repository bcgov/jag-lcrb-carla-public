using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.OData.OpenAPI;
using NJsonSchema;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using NSwag;
using System.Collections.Generic;

namespace odata2openapi
{
    class Program
    {
      

        // Small routine to allow the Org Book API to work with AutoRest.

        static void Main(string[] args)
        {
            var openAPIdata = File.ReadAllText("orgbook-swagger.json");
            
            var runner = SwaggerDocument.FromJsonAsync(openAPIdata);
            runner.Wait();
            var swaggerDocument = runner.Result;

            List<string> allops = new List<string>();
            Dictionary<string, SwaggerPathItem> itemsToRemove = new Dictionary<string, SwaggerPathItem>();
            // fix the operationIds.
            foreach (var operation in swaggerDocument.Operations)
            {
                var operationId = operation.Operation.OperationId;
                // strip out the v2.
                operationId = operationId.Replace("v2_", "");

                // now convert the data after the first underscore to camel case.
                
                operationId = string.Join("", operationId.Split(" ") .Select(i => Char.ToUpper(i[0]) + i.Substring(1)));

                operation.Operation.OperationId = operationId;

                    
            }

           var swagger = swaggerDocument.ToJson(SchemaType.Swagger2);

           File.WriteAllText("orgbook-swagger-adjusted.json", swagger);
            
        }
    }
}
