namespace FunctionApp.Integration.External.Common
{
    public interface IReferenceable<T>
    {
        IReference<T> ToReference();
    }
}
