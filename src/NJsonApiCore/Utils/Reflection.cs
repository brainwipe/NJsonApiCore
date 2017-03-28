using NJsonApi.Infrastructure;
using System;
using System.Collections;
using System.Reflection;

namespace NJsonApi.Utils
{
    public static class Reflection
    {
        public static Type GetObjectType(object objectGraph)
        {
            Type objectType = objectGraph.GetType();

            if (objectGraph is IMetaDataWrapper)
            {
                objectType = objectGraph.GetType().GetGenericArguments()[0];
            }

            if (typeof(IEnumerable).IsAssignableFrom(objectType))
            {
                if (objectType.IsArray)
                {
                    objectType = objectType.GetElementType();
                }
                else if (objectType.GetTypeInfo().IsGenericType)
                {
                    objectType = objectType.GetGenericArguments()[0];
                }
                
            }

            return objectType;
        }

        public static Type[] FromWithinGeneric(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsConstructedGenericType)
            {
                return type.GetGenericArguments();
            }
            else
            {
                return new Type[] { type };
            }
        }
    }
}
