namespace FunctionApp.Integration.External.Common
{
    public interface IIdentifiable<T>
    {
        string Id { get; }
    }
}
