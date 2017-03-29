using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJsonApiCore.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LinkedResourceAttribute : Attribute
    {
    }
}
