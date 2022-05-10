using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Tradefact.Api.Common.Extensions
{
    public static class EnumerationExtension
    {
        public static string GetDescription(this Enum value) =>
            value?.GetType()
                 .GetMember(value.ToString())
                 .FirstOrDefault()?
                 .GetCustomAttribute<DescriptionAttribute>()?
                 .Description;
    }
}
