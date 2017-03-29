using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NJsonApi.Conventions.Impl;
using NJsonApiCore.Infrastructure;

namespace NJsonApiCore.Conventions.Impl
{
    public class ComplexPropertyScanningConvention : DefaultPropertyScanningConvention
    {
        public override bool IsLinkedResource(PropertyInfo pi)
        {
            // This works, but doesn't apply naming convention to child properties
            var type = pi.PropertyType;
            bool isPrimitiveType = type.GetTypeInfo().IsPrimitive || type.GetTypeInfo().IsValueType || (type == typeof(string) || (type == typeof(DateTime)) || (type == typeof(TimeSpan)) || (type == typeof(DateTimeOffset)));
            return !isPrimitiveType && type.GetCustomAttribute<LinkedResourceAttribute>() != null;
        }
    }
}
