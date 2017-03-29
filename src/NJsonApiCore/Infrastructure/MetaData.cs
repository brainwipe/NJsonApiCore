using System;
using System.Collections.Generic;

namespace NJsonApi.Infrastructure
{
    public class MetaData : IMetaData
    {
        private Dictionary<string, object> _metaData = null;

        public Dictionary<string, object> GetMetaData()
        {
            EnsureMetadataInstance();
            return _metaData;
        }

        public void Add(string key, object value)
        {
            EnsureMetadataInstance();
            _metaData.Add(key, value);
        }

        private void EnsureMetadataInstance()
        {
            if (_metaData == null)
                _metaData = new Dictionary<string, object>();
        }
    }
}