using NJsonApi.Serialization.Representations.Relationships;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Serialization.Representations
{
    public interface IRelationship
    {
        RelationshipLinks Links { get; set; }

        IResourceLinkage Data { get; set; }

        Dictionary<string, string> Meta { get; set; }
    }
}