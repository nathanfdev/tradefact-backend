namespace FunctionApp.Integration.External.Common
{
    public class ResourceIdentifier<T> : ResourceIdentifier, IReference<T>, IKeyReferencable<T>
    {
        public new ReferenceTypeId TypeId => this.GetResourceType();

        public IReference<T> ToReference()
        {
            return this;
        }
    }
}
