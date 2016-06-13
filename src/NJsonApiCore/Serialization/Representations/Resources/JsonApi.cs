using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Serialization.Representations.Resources
{
    public class JsonApi
    {
        [JsonProperty(PropertyName = "version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version => "1.0";
    }
}