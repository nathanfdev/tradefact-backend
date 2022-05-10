using System;

namespace FunctionApp.Integration.External.Common
{
    public class ResourceTypeAttribute : Attribute
    {
        public ResourceTypeAttribute(ReferenceTypeId value)
        {
            this.Value = value;
        }

        public ReferenceTypeId Value { get; private set; }
    }
}
