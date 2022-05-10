namespace FunctionApp.Integration.External.Common
{
    public class Reference<T> : Reference, IReference<T>
    {
        public virtual T Obj { get; set; }
        public new ReferenceTypeId TypeId => this.GetResourceType();

        public IReference<T> ToReference()
        {
            return this;
        }
    }
}
