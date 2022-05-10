namespace FunctionApp.Integration.External.Common
{
    public interface IReference<T> : IReferenceable<T>, IIdentifiable<T>
    {
        ReferenceTypeId TypeId { get; }
        string Key { get; }
    }
}
