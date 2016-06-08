using Newtonsoft.Json;
using System;

namespace NJsonApi.Serialization.Converters
{
    internal class SerializationAwareConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ISerializationAware).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ((ISerializationAware)value).Serialize(writer);
        }
    }
}