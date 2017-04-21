using NJsonApi.Serialization.Representations;
using System;
using System.Collections.Generic;

namespace NJsonApi.Infrastructure
{
    public interface ITopLevelDocument
    {
        // returns top level resource
        object Value { get; }
        // returns top level reosurce type (in order to cache it)
        Type ValueType { get; }

        // below, other top level document data
        MetaData GetMetaData();
        Dictionary<string, ILink> Links { get; }
    }
}
