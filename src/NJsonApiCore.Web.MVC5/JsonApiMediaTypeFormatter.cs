using Newtonsoft.Json;
using NJsonApi;
using NJsonApi.Infrastructure;
using NJsonApi.Serialization;
using NJsonApi.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace NJsonApiCore.Web.MVC5
{
    public class JsonApiMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        private readonly IConfiguration configuration;
        private readonly JsonSerializer jsonSerializer;

        public JsonApiMediaTypeFormatter(IConfiguration configuration, JsonSerializer jsonSerializer)
        {
            this.configuration = configuration;
            this.jsonSerializer = jsonSerializer;
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(configuration.DefaultJsonApiMediaType));
            SupportedEncodings.Add(new UTF8Encoding(false, true));
        }

        public override bool CanReadType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Delta<>);
        }

        public override bool CanWriteType(Type type)

        {
            if (type == typeof(CompoundDocument))
            {
                return true;
            }

            if (type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(MetaDataWrapper<>))
                return configuration.IsResourceRegistered(type.GetGenericArguments()[0]);

            if (type == typeof(HttpError))
            {
                return true;
            }

            if ((typeof(Exception).IsAssignableFrom(type)))
            {
                return true;
            }

            return configuration.IsResourceRegistered(type);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var textWriter = new StreamWriter(writeStream))
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonSerializer.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            using (var reader = new StreamReader(readStream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var updateDocument = jsonSerializer.Deserialize(jsonReader, typeof(UpdateDocument)) as UpdateDocument;

                return updateDocument;
            }
        }
    }
}