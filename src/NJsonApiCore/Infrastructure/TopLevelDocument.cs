using NJsonApi.Serialization.Representations;
using System;
using System.Collections.Generic;

namespace NJsonApi.Infrastructure
{
    public class TopLevelDocument<T> : MetaDataContainer, ITopLevelDocument
    {
        private Type _valueType = typeof(T);

        public Dictionary<string, ILink> Links { get; } = new Dictionary<string, ILink>();

        object ITopLevelDocument.Value { get { return this.Value; } }

        public T Value { get; private set; }

        public Type ValueType { get { return _valueType; } }

        public TopLevelDocument(T value)
        {
            Value = value;
        }
    }
}
