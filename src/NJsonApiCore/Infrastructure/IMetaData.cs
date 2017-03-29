using System;
using System.Collections.Generic;

namespace NJsonApi.Infrastructure
{
    public interface IMetaData
    {
        Dictionary<string, object> GetMetaData();
        void Add(string key, object value);
    }
}