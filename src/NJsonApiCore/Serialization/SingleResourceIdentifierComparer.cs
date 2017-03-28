using System.Collections.Generic;
using NJsonApi.Serialization.Representations.Relationships;

namespace NJsonApi.Serialization
{
    public sealed class SingleResourceIdentifierComparer : IEqualityComparer<SingleResourceIdentifier>
    { 
        public bool Equals(SingleResourceIdentifier x, SingleResourceIdentifier y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return string.Equals(x.Id, y.Id) && string.Equals(x.Type, y.Type);
        }

        public int GetHashCode(SingleResourceIdentifier obj)
        {
            unchecked
            {
                return ((obj.Id != null ? obj.Id.GetHashCode() : 0) * 397) ^ (obj.Type != null ? obj.Type.GetHashCode() : 0);
            }
        }
    }
}
