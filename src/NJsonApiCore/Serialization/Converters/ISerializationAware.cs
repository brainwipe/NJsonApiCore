using Newtonsoft.Json;

namespace NJsonApi.Serialization.Converters
{
    internal interface ISerializationAware
    {
        void Serialize(JsonWriter writer);
    }
}