using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.OData;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi;
using Newtonsoft.Json;

// online validator that seems to work - https://apitools.dev/swagger-parser/online/

namespace odata2openapi
{
    class Program
    {
        static string solutionPrefix;

        static string GetDynamicsMetadata (IConfiguration Configuration )
        {
            string dynamicsOdataUri = Configuration["DYNAMICS_ODATA_URI"];
            string aadTenantId = Configuration["DYNAMICS_AAD_TENANT_ID"];
            string serverAppIdUri = Configuration["DYNAMICS_SERVER_APP_ID_URI"];
            string clientKey = Configuration["DYNAMICS_CLIENT_KEY"];
            string clientId = Configuration["DYNAMICS_CLIENT_ID"];
            string ssgUsername = Configuration["SSG_USERNAME"];
            string ssgPassword = Configuration["SSG_PASSWORD"];

            // ensure the last character of the uri is a slash.
            if (dynamicsOdataUri[dynamicsOdataUri.Length - 1] != '/')
            {
                dynamicsOdataUri += "/";
            }

            var webRequest = WebRequest.Create(dynamicsOdataUri + "$metadata");
            HttpWebRequest request = (HttpWebRequest)webRequest;

            if (string.IsNullOrEmpty(ssgUsername) || string.IsNullOrEmpty(ssgPassword))
            {
                var authenticationContext = new AuthenticationContext(
                "https://login.windows.net/" + aadTenantId);
                ClientCredential clientCredential = new ClientCredential(clientId, clientKey);
                var task = authenticationContext.AcquireTokenAsync(serverAppIdUri, clientCredential);
                task.Wait();
                AuthenticationResult authenticationResult = task.Result;
                request.Headers.Add("Authorization", authenticationResult.CreateAuthorizationHeader());
            }
            else
            {
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(ssgUsername + ":" + ssgPassword));
                request.Headers.Add("Authorization", "Basic " + encoded);
            }

            string result = null;

            request.Method = "GET";
            request.PreAuthenticate = true;
            //request.Accept = "application/json;odata=verbose";
            //request.ContentType =  "application/json";

            // we need to add authentication to a HTTP Client to fetch the file.
            using (
                MemoryStream ms = new MemoryStream())
            {
                request.GetResponse().GetResponseStream().CopyTo(ms);
                var buffer = ms.ToArray();
                result = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }

            return result;

        }
        /*
        static void FixProperty (JsonSchema4 item , string name)
        {
            if (item.Properties.Keys.Contains(name))
            {
                item.Properties.Remove(name);

                JsonProperty jsonProperty = new JsonProperty();
                jsonProperty.Type = JsonObjectType.String;
                jsonProperty.IsNullableRaw = true;
                KeyValuePair<string, JsonProperty> keyValuePair = new KeyValuePair<string, JsonProperty>(name, jsonProperty);

                item.Properties.Add(keyValuePair);

            }
        }
        */
        static void CheckProperties(OpenApiDocument swaggerDocument, List<string> itemsToKeep, OpenApiSchema item)
        {
            if (item.Reference != null)
            {
                string title = item.Reference.Id;
                if (title != null && !itemsToKeep.Contains(title))
                {
                    // recursive call.
                    AddSubItems(swaggerDocument, itemsToKeep, title);
                }
            }
            else
            {
                if (item.Type != null && (item.Type == "object" || item.Type == "array")
                    && item.Items?.Reference?.Id != null)
                {
                    string title = item.Items.Reference.Id;
                    if (title != null && !itemsToKeep.Contains(title))
                    {
                        // recursive call.
                        AddSubItems(swaggerDocument, itemsToKeep, title);
                    }
                }
            }
            if (item.Properties != null)
            {
                foreach (var property in item.Properties)
                {
                    if (property.Value.Reference != null)
                    {
                        string title = property.Value.Reference.Id;
                        if (title != null && !itemsToKeep.Contains(title))
                        {
                            // recursive call.
                            AddSubItems(swaggerDocument, itemsToKeep, title);
                        }
                    }
                    else
                    {
                        if ((property.Value.Type == "object" || property.Value.Type == "array")
                            && property.Value.Items?.Reference?.Id != null)
                        {
                            string id = property.Value.Items.Reference.Id;
                            if (id != null && !itemsToKeep.Contains(id))
                            {
                                // recursive call.
                                AddSubItems(swaggerDocument, itemsToKeep, id);
                            }
                        }
                    }
                }

            }
        }
        static void AddSubItems (OpenApiDocument swaggerDocument, List<string> itemsToKeep, string currentItem)
        {
            if (currentItem != null && ! itemsToKeep.Contains (currentItem) && swaggerDocument.Components.Schemas.ContainsKey(currentItem))
            {                
                itemsToKeep.Add(currentItem);
                //Console.WriteLine($"Added {currentItem}");

                OpenApiSchema item = swaggerDocument.Components.Schemas[currentItem];

                CheckProperties(swaggerDocument, itemsToKeep, item);
            
                if (item.AllOf != null)
                {
                    foreach (var allOfItem in item.AllOf)
                    {
                        CheckProperties(swaggerDocument, itemsToKeep, allOfItem);
                    }
                }

            }
        }

     
        static void MergeSubItems (Dictionary <string, OpenApiSchema> dictionary, OpenApiSchema item)
        {
            if (item.AllOf != null)
            {
                foreach (var allOfItem in item.AllOf)
                {
                    MergeSubItems(dictionary, allOfItem);
                }
            }
            if (item.Properties != null)
            {
                foreach (var property in item.Properties)
                {
                    dictionary.Add(property.Key, property.Value);
                }
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a solution prefix");                
            }
            else
            {
                solutionPrefix = args[0];
            }

            bool generatePaths = false;

            if (args.Length > 1)
            {
                if (args[1] == "generatePaths")
                {
                    generatePaths = true;
                }
            }

            // Feature flags
            
            // True if we use the Autorest Odata extension
            bool enableOdataExtension = false;

            // True if we get metadata from Dynamics
            bool getMetadata = true;

            // True if we use strings instead of guids primary keys.
            bool useStringForGuid = true;

            bool disableCustomErrorClass = true;


            List<string> defsToKeep = new List<string>();
            
            
            

            // start by getting secrets.
            var builder = new ConfigurationBuilder()                
                .AddEnvironmentVariables();

            builder.AddUserSecrets<Program>();           
            var Configuration = builder.Build();
            string csdl;
            // get the metadata.
            if (getMetadata)
            {
                try
                {
                    csdl = GetDynamicsMetadata(Configuration);
                    File.WriteAllText("dynamics-metadata.xml", csdl);
                }
                catch (System.Net.WebException)
                {
                    csdl = File.ReadAllText("dynamics-metadata.xml");
                }
                
            }
            else
            {
                csdl = File.ReadAllText("dynamics-metadata.xml");
            }               

            // fix the csdl.

            csdl = csdl.Replace("ConcurrencyMode=\"Fixed\"", "");

            IEdmModel model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());

            // fix dates.
            /*
            OpenApiTarget target = OpenApiTarget.Json;
            OpenApiWriterSettings settings = new OpenApiWriterSettings
            {
                BaseUri = new Uri(Configuration["DYNAMICS_ODATA_URI"])
            };
            */
           
            string swagger = null;

            OpenApiConvertSettings openApiSettings = new OpenApiConvertSettings
                {
                    // configuration
                };
                OpenApiDocument swaggerDocument = model.ConvertToOpenApi(openApiSettings);
                
                if (generatePaths)
                {
                    List<string> paths = new List<string>();
                    foreach (var path in swaggerDocument.Paths)
                    {                        

                        string subPath = path.Key.Substring(path.Key.LastIndexOf("/") + 1);
                        if (subPath.Contains("("))
                        {
                            subPath = subPath.Substring(0, subPath.IndexOf("("));                     
                        }
                    
                        string pathSample = path.Key.Substring(1, subPath.Length);
                        if (pathSample != subPath)
                        {
                            paths.Add(path.Key);
                        }


                    }
                    File.WriteAllText("paths-excluded.json", JsonConvert.SerializeObject(paths));
                }
                else {
                List<string> itemsToRemove = new List<string>();
                if (File.Exists ("paths-excluded.json"))
                {
                    string pathsExcluded = File.ReadAllText("paths-excluded.json");
                    itemsToRemove = JsonConvert.DeserializeObject<List<string>>(pathsExcluded);
                }
                

                List<string> allops = new List<string>();
                
                // fix the operationIds.
                foreach (var path in swaggerDocument.Paths)
                {
                    if (itemsToRemove.Contains (path.Key))
                    {
                        continue;
                    }
                    if (path.Key.Contains("transactioncurrencyid"))
                    {
                        itemsToRemove.Add(path.Key);
                        continue;
                    }
                    if (path.Key.Contains("scheduledprocessexecution"))
                    {
                        itemsToRemove.Add(path.Key);
                        continue;
                    }

                    // determine if this is a get by key, or a get by key with additional .
                    string subPath = "";

                    string temp = path.Key.Substring(path.Key.LastIndexOf("/") + 1);
                    if (temp.Contains("("))
                    {
                        temp = temp.Substring(0, temp.IndexOf("("));
                        string pathSample = path.Key.Substring(1, temp.Length);
                        if (pathSample != temp)
                        {
                            subPath = temp;
                        }
                        
                        subPath += "ByKey";
                    }

                    OpenApiTag firstTag = null; // operation.Value.Tags.FirstOrDefault();
                    string firstTagLower = "";
                    
                    if (firstTag == null)
                    {
                        firstTag = new OpenApiTag() { Name = temp };
                    }                    

                    string prefix = "Unknown";

                    if (firstTag != null)
                    {
                        bool ok2Delete = true;
                        firstTagLower = firstTag.Name.ToLower();
                        //Console.Out.WriteLine(firstTagLower);
                        if (firstTagLower.Equals("contacts") ||
                            firstTagLower.Equals("accounts") ||
                            firstTagLower.Equals("invoices") ||
                            firstTagLower.Equals("leads") ||
                            firstTagLower.Equals("lists") ||
                            firstTagLower.Equals("sharepointsites") ||
                            firstTagLower.Equals("savedqueries") ||
                            firstTagLower.Equals("sharepointdocumentlocations") ||
                            firstTagLower.Equals("entitydefinitions") ||
                            firstTagLower.Equals("globaloptionsetdefinitions")
                            )
                        {
                            ok2Delete = false;
                            //Console.Out.WriteLine($"NOT ok to delete {firstTagLower}");
                        }

                        if (!firstTagLower.StartsWith("msdyn") && !firstTagLower.StartsWith("abs_") && firstTagLower.IndexOf(solutionPrefix) != -1)
                        {
                            //Console.Out.WriteLine($"NOT ok to delete {firstTagLower}");
                            ok2Delete = false;                        
                        }

                        if (ok2Delete)
                        {
                            //Console.Out.WriteLine($"ok to delete {firstTagLower}");
                            if (!itemsToRemove.Contains(path.Key))
                            {
                                itemsToRemove.Add(path.Key);
                            }
                            continue;
                        }

                        if (!allops.Contains(firstTag.Name))
                        {
                            allops.Add(firstTag.Name);
                        }
                        prefix = firstTagLower;
                        // Capitalize the first character.

                        if (prefix.Length > 0  && prefix.Length > solutionPrefix.Length)
                        {
                            if (prefix.ToUpper().Substring(0,solutionPrefix.Length) == solutionPrefix.ToUpper())
                            {
                                prefix = prefix.Substring(solutionPrefix.Length);
                            }
                            prefix = prefix.Substring(0,1).ToUpper() + prefix.Substring(1);
                        }
                        // remove any underscores.
                        prefix = prefix.Replace("_", "");
                        
                    }

                    foreach (var operation in path.Value.Operations)
                    {

                        if (!firstTagLower.StartsWith("msdyn") && !firstTagLower.StartsWith("abs_") && firstTagLower.IndexOf(solutionPrefix) != -1)
                        {
                            firstTagLower = firstTagLower.Replace($"{solutionPrefix}_", "");
                            firstTagLower = firstTagLower.Replace(solutionPrefix, "");
                            operation.Value.Tags.Clear();
                            operation.Value.Tags.Add(new OpenApiTag() { Name = firstTagLower });
                        }

                        string suffix = "";


                        switch (operation.Key)
                        {
                            case OperationType.Post:
                                suffix = "Create";
                                // for creates we also want to add a header parameter to ensure we get the new object back.
                                OpenApiParameter swaggerParameter = new OpenApiParameter()
                                {
                                    Schema = new OpenApiSchema() { Type = "string", Default = new OpenApiString("return=representation") },
                                    Name = "Prefer",
                                    Description = "Required in order for the service to return a JSON representation of the object.",
                                    In = ParameterLocation.Header
                                };
                                operation.Value.Parameters.Add(swaggerParameter);
                                break;

                            case OperationType.Patch:
                                suffix = "Update";
                                if (subPath == "ByKey")
                                {
                                    subPath = "";
                                }
                                break;

                            case OperationType.Put:
                                suffix = "Put";
                                if (subPath == "ByKey")
                                {
                                    subPath = "";
                                }
                                break;

                            case OperationType.Delete:
                                suffix = "Delete";
                                if (subPath == "ByKey")
                                {
                                    subPath = "";
                                }
                                break;

                            case OperationType.Get:
                                suffix = "Get";                                
                                break;
                        }

                        if (suffix.Length >= solutionPrefix.Length && suffix.ToUpper().Substring(0, solutionPrefix.Length) == solutionPrefix.ToUpper())
                        {
                            suffix = suffix.Substring(solutionPrefix.Length);
                        }                        

                        operation.Value.OperationId = prefix + "_" + suffix;

                        if (! firstTag.Name.Contains(subPath))
                        {
                            operation.Value.OperationId += subPath;
                        }

                        string operationDef = null;
                        // adjustments to response

                        foreach (var response in operation.Value.Responses)
                        {
                            var val = response.Value;
                            if (response.Key == "default")
                            {
                                if (string.IsNullOrEmpty(response.Value.Description))
                                {
                                    response.Value.Description = "OData Error";
                                };

                                if (disableCustomErrorClass && response.Value.Reference != null)
                                {
                                    response.Value.Reference = null;
                                }                                
                            }

                            if (val != null)
                            {
                                bool hasValue = false;
                                foreach (var schema in val.Content)
                                {
                                    foreach (var property in schema.Value.Schema.Properties)
                                    {
                                        if (property.Key.Equals("value"))
                                        {
                                            hasValue = true;
                                            break;
                                        }
                                    }
                                    if (hasValue)
                                    {
                                        var newSchema = schema.Value.Schema.Properties["value"];
                                        string resultName;
                                        string itemName;

                                        if (newSchema.Type == "array")
                                        {
                                            itemName = newSchema.Items.Reference.Id;
                                            operationDef = itemName;
                                            resultName = $"{itemName}Collection";
                                        }
                                        else
                                        {
                                            itemName = "!ERR";
                                            resultName = "!ERR";
                                        }



                                        if (!swaggerDocument.Components.Schemas.ContainsKey(resultName))
                                        {
                                            // move the inline schema to defs.
                                            swaggerDocument.Components.Schemas.Add(resultName, schema.Value.Schema);

                                            //var newSchema = swaggerDocument.Components.Schemas[resultName].Properties["value"];
                                            if (newSchema.Type == "array")
                                            {
                                                newSchema.Items = new OpenApiSchema() { Reference = new OpenApiReference() { Id = itemName, Type = ReferenceType.Schema }, Type = "none" };
                                                AddSubItems(swaggerDocument, defsToKeep, itemName);
                                            }
                                            AddSubItems(swaggerDocument, defsToKeep, resultName);
                                            
                                        }

                                        schema.Value.Schema = new OpenApiSchema { Reference = new OpenApiReference() { Id = resultName, Type = ReferenceType.Schema }, Type = "none" };
                                        // val.Reference = new OpenApiReference() { Id = resultName, Type = ReferenceType.Schema, ExternalResource = null };
                                    }
                                    else
                                    {
                                        if (schema.Value.Schema.Reference != null)
                                        {
                                            operationDef = schema.Value.Schema.Reference.Id;
                                            AddSubItems(swaggerDocument, defsToKeep, operationDef);
                                        }                                 
                                    }
                                }                                
                            }
                        }





                        // adjustments to operation parameters                        
                        if (enableOdataExtension)
                        {
                            string[] oDataParameters = { "top", "skip", "search", "filter", "count", "$orderby", "$select", "$expand" };

                            List<OpenApiParameter> parametersToRemove = new List<OpenApiParameter>();

                            foreach (var oDataParameter in oDataParameters)
                            {
                                foreach (var parameter in operation.Value.Parameters)
                                {
                                    if (parameter.Name == oDataParameter)
                                    {
                                        parametersToRemove.Add(parameter);
                                    }

                                    if (parameter.Reference != null && parameter.Reference.Id == oDataParameter)
                                    {
                                        parametersToRemove.Add(parameter);
                                    }

                                }
                            }
                            foreach (var parameter in parametersToRemove)
                            {
                                operation.Value.Parameters.Remove(parameter);
                            }

                        }


                        operation.Value.Extensions.Clear();


                        if (operationDef != null  && enableOdataExtension)
                        {
                            operation.Value.Extensions.Add("x-ms-odata", new OpenApiString($"#/definitions/{operationDef}"));
                            AddSubItems(swaggerDocument, defsToKeep, operationDef);
                        }                        

                        foreach (var parameter in operation.Value.Parameters)
                        {
                            if (parameter.Reference != null)
                            {
                                if (parameter.Reference.Id == "top" || parameter.Reference.Id == "skip")
                                {
                                    parameter.Name = $"${parameter.Reference.Id}";
                                    parameter.Reference = null;                                    
                                    parameter.In = ParameterLocation.Query;                                    
                                    parameter.Schema = new OpenApiSchema()
                                    {
                                        Type = "integer"

                                    };
                                }
                                else if (parameter.Reference.Id == "search" || parameter.Reference.Id == "filter")
                                {
                                    parameter.Name = $"${parameter.Reference.Id}";
                                    parameter.Reference = null;
                                    parameter.In = ParameterLocation.Query;
                                    parameter.Schema = new OpenApiSchema()
                                    {
                                        Type = "string"

                                    };
                                }
                                else if (parameter.Reference.Id == "count")
                                {
                                    parameter.Name = $"${parameter.Reference.Id}";
                                    parameter.Reference = null;
                                    parameter.In = ParameterLocation.Query;
                                    parameter.Schema = new OpenApiSchema()
                                    {
                                        Type = "boolean"

                                    };
                                }
                            }
                            else
                            {
                                string name = parameter.Name;
                                if (name == null)
                                {
                                    name = parameter.Name;
                                }

                                if (name != null)
                                {
                                    if (name == "$top" || name == "$skip")
                                    {
                                        parameter.In = ParameterLocation.Query;
                                        parameter.Reference = null;
                                        parameter.Schema = new OpenApiSchema()
                                        {
                                            Type = "integer"

                                        };
                                    }
                                    if (name == "$search" || name == "$filter")
                                    {
                                        parameter.In = ParameterLocation.Query;
                                        parameter.Reference = null;
                                        parameter.Schema = new OpenApiSchema()
                                        {
                                            Type = "string"
                                        };
                                    }
                                    if (name == "$orderby" || name == "$select" || name == "$expand")
                                    {
                                        parameter.In = ParameterLocation.Query;
                                        parameter.Reference = null;
                                        parameter.Schema = new OpenApiSchema()
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema()
                                            {
                                                Type = "string"
                                            }

                                        };
                                        parameter.Extensions.Add("collectionFormat", new OpenApiString("csv"));                                        

                                    }
                                    if (name == "$count")
                                    {
                                        parameter.In = ParameterLocation.Query;
                                        parameter.Reference = null;
                                        parameter.Schema = new OpenApiSchema()
                                        {
                                            Type = "boolean"
                                        };
                                    }
                                    if (name == "If-Match")
                                    {
                                        parameter.Reference = null;
                                        parameter.Schema = new OpenApiSchema()
                                        {
                                            Type = "string"
                                        };
                                    }

                                    if (parameter.Extensions != null && parameter.Extensions.ContainsKey("x-ms-docs-key-type"))
                                    {
                                        parameter.Extensions.Remove("x-ms-docs-key-type");
                                    }

                                    if (string.IsNullOrEmpty(parameter.Name))
                                    {
                                        parameter.Name = name;
                                    }

                                }

                            }                            

                            // get rid of style if it exists.
                            if (parameter.Style != ParameterStyle.Simple)
                            {
                                parameter.Style = ParameterStyle.Simple;
                            }

                            // get rid of guid type if it exists

                            if (parameter.Schema?.Format != null && useStringForGuid && parameter.Schema.Format == "uuid")
                            {
                                parameter.Schema.Format = null;
                                parameter.Schema.Pattern = null;
                            }

                            // may need to clear unique items here.

                            // align the schema if it exists.
                            if (parameter.Schema != null && parameter.Schema.Items != null)
                            {

                                var schema = parameter.Schema;
                                if (schema.Type == "array" && parameter.Style == null)
                                {

                                    // may be a good idea to set collectionFormat to csv here.  It is also set below.

                                    parameter.Style = ParameterStyle.Simple;
                                }
                            }
                        }

                    }

                }

                foreach (var opDelete in itemsToRemove)
                {
                    swaggerDocument.Paths.Remove(opDelete);
                }

                foreach (var path in swaggerDocument.Paths)
                {
                    foreach (var value in path.Value.Operations)
                    {
                        foreach (var response in value.Value.Responses)
                        {
                            if (response.Value.Reference != null)
                            {
                                var schema = response.Value.Reference;
                                if (!string.IsNullOrEmpty(schema.Id) && (schema.Type.Value.GetDisplayName() == "array" || schema.Type.Value.GetDisplayName() == "object"))
                                {
                                    string title = schema.Id;
                                    AddSubItems(swaggerDocument, defsToKeep, title);
                                }
                            }
                        }
                        foreach (var parameter in value.Value.Parameters)
                        {
                            if (parameter.Schema != null && parameter.Schema.Reference != null)
                            {
                                var schema = parameter.Schema.Reference;
                                if (!string.IsNullOrEmpty(schema.Id) && (schema.Type.Value.GetDisplayName() == "array" || schema.Type.Value.GetDisplayName() == "object"))
                                {
                                    AddSubItems(swaggerDocument, defsToKeep, schema.Id);
                                }
                            }
                        }

                    }
                }

               

                // reverse the items to keep.

                List<string> defsToRemove = new List<string>();


                foreach (var definition in swaggerDocument.Components.Schemas)
                {
                    if (
                        !definition.Key.Contains("odata.error") &&
                        !definition.Key.Contains("crmbaseentity") &&
                        !definition.Key.ToLower().Contains("optionmetadata") &&
                        !defsToKeep.Contains(definition.Key))
                    {
                        defsToRemove.Add(definition.Key);                        
                    }
                }                
                
                foreach (string defToRemove in defsToRemove)
                {
                    //Console.Out.WriteLine($"Remove: {defToRemove}");
                    if (!string.IsNullOrEmpty (defToRemove) && swaggerDocument.Components.Schemas.ContainsKey (defToRemove))
                    {
                        swaggerDocument.Components.Schemas.Remove(defToRemove);
                    }
                    
                }
                /*
                List<string> responsesToRemove = new List<string>();

                
                foreach (string responseToRemove in responsesToRemove)
                {
                    Console.Out.WriteLine($"Remove Response: {responseToRemove}");
                
                    if (!string.IsNullOrEmpty(responseToRemove) && swaggerDocument.Components.Responses.ContainsKey(responseToRemove))
                    {
                        swaggerDocument.Components.Responses.Remove(responseToRemove);
                    }
                   

                }

                 */

                /*
                 * Cleanup definitions.                 
                 */

                foreach (var definition in swaggerDocument.Components.Schemas)
                {
                    
                    if (definition.Value.Description == null)
                    {
                        definition.Value.Description = definition.Key;
                    }


                    // consolidate AllOf
                    if (definition.Value.AllOf != null && definition.Value.Type == null)
                    {
                        definition.Value.Type = "object";
                    }
                    
                    var dictionary = new Dictionary<string, OpenApiSchema>();
                    MergeSubItems(dictionary, definition.Value);
                    definition.Value.Properties = dictionary;
                    if (definition.Value.AllOf != null)
                    {
                        definition.Value.AllOf.Clear();
                    }

                    // Clear out the example if it exists

                    if (definition.Value.Example != null)
                    {
                        definition.Value.Example = null;
                    }
                    /*
                    if (definition.Value.Title != null)
                    {
                        definition.Value.Title = null;
                    }

                    if (definition.Value.Enum != null)
                    {
                        definition.Value.Enum.Clear();
                    }
                    */

                    if (definition.Value != null && definition.Value.Properties != null)
                    {
                        foreach (var property in definition.Value.Properties)
                        {
                            // convert all dates to datetimeoffset.
                            // special handling of the Dynamics "DATE (YYYY-MM-DD)" fields will need to be done with extensions.
                            if (property.Value.Format != null && property.Value.Format == "date")
                            {
                                property.Value.Format = "date-time";
                            }
                            if (property.Value.Type == null)
                            {
                                property.Value.Type = "string";
                            }

                            // fix for doubles
                            if (property.Value != null && property.Value.Format != null && property.Value.Format.Equals("double"))
                            {
                                property.Value.Format = "decimal";
                                property.Value.Type = "number";
                            }                            
                            if (property.Key.Equals("totalamount"))
                            {
                                property.Value.Type = "number";
                                property.Value.Format = "decimal";
                            }

                            if (property.Key.Equals("versionnumber"))
                            {
                                // clear oneof.
                                if (property.Value.OneOf != null)
                                {
                                    property.Value.OneOf.Clear();
                                }

                                // force to string.
                                property.Value.Type = "string";
                            }

                            if (property.Value.Minimum != null)
                            {
                                property.Value.Minimum = null;
                            }
                            if (property.Value.Maximum != null)
                            {
                                property.Value.Maximum = null;
                            }
                            if (property.Value.Pattern != null)
                            {
                                property.Value.Pattern = null;
                            }

                            
                            if (property.Value.Format != null && property.Value.Format == "uuid" && useStringForGuid)
                            {
                                property.Value.Format = null;
                            }

                        }
                    }
                }


                // remove extra tags.
                swaggerDocument.Tags.Clear();

                // cleanup parameters.
                //swaggerDocument.Components.Parameters.Clear();


                //**************************************

                // fix for "odata.error.main" - innererror property.

                swaggerDocument.Components.Schemas["odata.error.main"].Properties.Remove("innererror");

                // fix for two entities that have links to everything else - this causes massive spikes in memory consumption.

                swaggerDocument.Components.Schemas.Remove("Microsoft.Dynamics.CRM.transactioncurrency");
                swaggerDocument.Components.Schemas.Remove("Microsoft.Dynamics.CRM.syncerror");
                
                Dictionary<string, OpenApiSchema> props = new Dictionary<string, OpenApiSchema>();
                props.Add("nil", new OpenApiSchema() { Type = "string" });
                swaggerDocument.Components.Schemas.Add("Microsoft.Dynamics.CRM.transactioncurrency", new OpenApiSchema() { Properties = props});
                swaggerDocument.Components.Schemas.Add("Microsoft.Dynamics.CRM.syncerror", new OpenApiSchema() { Properties = props });


                // swagger = swaggerDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0); // ToJson(SchemaType.Swagger2);
                swagger = swaggerDocument.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);
                
            }
            
            File.WriteAllText("dynamics-swagger.json", swagger);
            
        }
    }
}
