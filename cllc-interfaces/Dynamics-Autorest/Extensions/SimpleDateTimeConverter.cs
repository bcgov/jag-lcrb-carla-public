using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gov.Lclb.Cllb.Interfaces
{
    
    public class SimpleDateTimeConverter : Newtonsoft.Json.JsonConverter
    {        
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            
            return new DateTimeOffset? (DateTime.Parse(reader.Value.ToString()));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTimeOffset dt = ((DateTimeOffset?)value).Value;
            DateTimeOffset newDt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);            
            writer.WriteValue(newDt.ToString("yyyy-MM-dd"));

        }
    }
    
}
