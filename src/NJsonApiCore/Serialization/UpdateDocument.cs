using Newtonsoft.Json;
using NJsonApi.Serialization.Representations.Resources;
using System.Collections.Generic;

namespace NJsonApi.Serialization
{
    public class UpdateDocument
    {
        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public SingleResource Data { get; set; }

        [JsonProperty(PropertyName = "meta", Required = Required.Default)]
        public Dictionary<string, object> MetaData { get; set; }
    }
}