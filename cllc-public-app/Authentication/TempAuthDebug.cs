using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Authentication
{
    // TEMP-DEBUG: delete this file and every TempAuthDebug call once the BCeID name investigation is finished.
    public static class TempAuthDebug
    {
        public const string Tag = "[TEMP-DEBUG-BCEID-NAMES]";

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = { new EntityConverter() }
        };

        public static string Json(object value)
        {
            try
            {
                return JsonConvert.SerializeObject(value, Settings);
            }
            catch (Exception ex)
            {
                return $"<serialization failed: {ex.Message}>";
            }
        }

        // Dataverse entities serialize poorly by default, so flatten them to their attributes.
        private class EntityConverter : JsonConverter<Entity>
        {
            public override void WriteJson(JsonWriter writer, Entity value, JsonSerializer serializer)
            {
                var attributes = new Dictionary<string, object>();
                foreach (var attribute in value.Attributes)
                {
                    attributes[attribute.Key] = attribute.Value switch
                    {
                        EntityReference reference => $"{reference.LogicalName}:{reference.Id}",
                        OptionSetValue option => option.Value,
                        Money money => money.Value,
                        _ => attribute.Value
                    };
                }

                serializer.Serialize(writer, new { value.LogicalName, value.Id, Attributes = attributes });
            }

            public override Entity ReadJson(JsonReader reader, Type objectType, Entity existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }

            public override bool CanRead => false;
        }

        public static void Log(ILogger logger, string function, string message)
        {
            logger.LogInformation("{Tag}[{Function}] {Message}", Tag, function, message);
        }

        // All headers are logged so unreferenced SiteMinder values are visible; only secret-bearing ones are redacted.
        public static void LogHeaders(ILogger logger, string function, IHeaderDictionary headers)
        {
            var redacted = new[] { "cookie", "authorization", "proxy-authorization", "x-csrf-token", "x-xsrf-token" };
            var values = headers.ToDictionary(
                h => h.Key,
                h => redacted.Contains(h.Key.ToLowerInvariant()) ? "<redacted>" : h.Value.ToString());
            Log(logger, function, "All request headers: " + Json(values));
        }
    }
}
