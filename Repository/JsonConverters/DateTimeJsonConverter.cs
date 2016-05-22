using System;
using Newtonsoft.Json;

namespace Repository.JsonConverters
{
    public class DateTimeJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dateTime = (DateTime)value;
            writer.WriteRawValue($"ISODate(\"{dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.FFFZ")}\")");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }
}
