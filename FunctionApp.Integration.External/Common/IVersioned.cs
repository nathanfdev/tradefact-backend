namespace FunctionApp.Integration.External.Common
{
    public interface IVersioned<T> : IIdentifiable<T>
    {
        int Version { get; }
    }
}
