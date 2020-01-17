using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.OrgbookService
{
    public class OrgBookUtils
    {
        public static (string, string) GetSchemaFromConfig(string licenceType)
        {
            using (StreamReader file = File.OpenText(@"CredentialSchemaConfig.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<Schema> schemas = (List<Schema>)serializer.Deserialize(file, typeof(List<Schema>));
                Schema schema = schemas.Find((obj) => obj.type == licenceType);

                return schema == null ? (null, null) : (schema.name, schema.version);
            }
        }
    }
}
