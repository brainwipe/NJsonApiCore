using System;
using System.Collections.Generic;

namespace NJsonApi.Infrastructure
{
    public interface IMetaDataWrapper
    {
        Dictionary<string, object> MetaData { get; }
        object GetValue();
    }

    public interface IMetaData
    {
        Dictionary<string, object> GetMetaData();
    }
}