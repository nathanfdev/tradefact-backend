namespace FunctionApp.Integration.External.Common
{
    public interface IKeyReferencable<T>
    {
        string Key { get; }
    }
}
