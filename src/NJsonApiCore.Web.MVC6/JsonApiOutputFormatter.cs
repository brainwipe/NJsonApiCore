using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace NJsonApi.Web.MVC6
{
    public class JsonApiOutputFormatter : JsonOutputFormatter
    {
        public JsonApiOutputFormatter(IConfiguration configuration)
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(configuration.DefaultJsonApiMediaType));
        }
    }
}