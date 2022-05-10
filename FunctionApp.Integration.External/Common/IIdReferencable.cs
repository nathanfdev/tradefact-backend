namespace FunctionApp.Integration.External.Common
{
    public interface IIdReferencable<T>
    {
        string Id { get; }
    }
}
