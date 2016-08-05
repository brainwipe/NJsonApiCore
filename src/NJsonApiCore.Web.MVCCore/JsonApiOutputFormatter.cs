using System.Buffers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace NJsonApi.Web.MVCCore
{
    public class JsonApiOutputFormatter : JsonOutputFormatter
    {
        public JsonApiOutputFormatter(IConfiguration configuration) : base(configuration.GetJsonSerializerSettings(), ArrayPool<char>.Shared)
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(configuration.DefaultJsonApiMediaType));
        }
    }
}