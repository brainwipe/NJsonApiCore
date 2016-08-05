using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NJsonApi
{
    public interface IConfiguration
    {
        string DefaultJsonApiMediaType { get; }

        void AddMapping(IResourceMapping resourceMapping);

        IResourceMapping GetMapping(Type type);

        IResourceMapping GetMapping(object objectGraph);

        JsonSerializer GetJsonSerializer();

        bool IsMappingRegistered(Type type);

        bool ValidateIncludedRelationshipPaths(string[] includedPaths, object objectGraph);

        bool ValidateAcceptHeader(string acceptsHeaders);

        string[] FindRelationshipPathsToInclude(string includeQueryParameter);

        JsonSerializerSettings GetJsonSerializerSettings();
    }
}