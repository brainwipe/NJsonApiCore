using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonApi.Exceptions;
using NJsonApi.Serialization.Converters;
using NJsonApi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NJsonApi
{
    public class Configuration : IConfiguration
    {
        private readonly Dictionary<string, IResourceMapping> resourcesMappingsByResourceType = new Dictionary<string, IResourceMapping>();
        private readonly Dictionary<Type, IResourceMapping> resourcesMappingsByType = new Dictionary<Type, IResourceMapping>();

        private readonly JsonSerializerSettings jsonSerializerSettings;

        public Configuration()
        {
            jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            jsonSerializerSettings.Converters.Add(new RelationshipDataConverter());
            jsonSerializerSettings.Converters.Add(new StringEnumConverter() { CamelCaseText = true });
#if DEBUG
            jsonSerializerSettings.Formatting = Formatting.Indented;
#endif
        }

        public string DefaultJsonApiMediaType => "application/vnd.api+json";

        public void AddMapping(IResourceMapping resourceMapping)
        {
            resourcesMappingsByResourceType[resourceMapping.ResourceType] = resourceMapping;
            resourcesMappingsByType[resourceMapping.ResourceRepresentationType] = resourceMapping;
        }

        public bool IsMappingRegistered(Type type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type) && type.GetTypeInfo().IsGenericType)
            {
                return resourcesMappingsByType.ContainsKey(type.GetGenericArguments()[0]);
            }

            return resourcesMappingsByType.ContainsKey(type);
        }

        public IResourceMapping GetMapping(Type type)
        {
            IResourceMapping mapping;
            resourcesMappingsByType.TryGetValue(type, out mapping);
            return mapping;
        }

        public IResourceMapping GetMapping(object objectGraph)
        {
            return GetMapping(Reflection.GetObjectType(objectGraph));
        }

        public bool ValidateIncludedRelationshipPaths(string[] includedPaths, object objectGraph)
        {
            var mapping = GetMapping(objectGraph);
            if (mapping == null)
            {
                throw new MissingMappingException(Reflection.GetObjectType(objectGraph));
            }
            return mapping.ValidateIncludedRelationshipPaths(includedPaths);
        }

        public JsonSerializerSettings GetJsonSerializerSettings()
        {
            return jsonSerializerSettings;
        }

        public JsonSerializer GetJsonSerializer()
        {
            var jsonSerializer = JsonSerializer.Create(GetJsonSerializerSettings());
            return jsonSerializer;
        }

        public bool ValidateAcceptHeader(string acceptsHeaders)
        {
            if (string.IsNullOrEmpty(acceptsHeaders))
            {
                return true;
            }

            return acceptsHeaders
                .Split(',')
                .Select(x => x.Trim())
                .Any(x =>
                    x == "*/*" ||
                    x == DefaultJsonApiMediaType);
        }

        public string[] FindRelationshipPathsToInclude(string includeQueryParameter)
        {
            return string.IsNullOrEmpty(includeQueryParameter) ? new string[0] : includeQueryParameter.Split(',');
        }
    }
}