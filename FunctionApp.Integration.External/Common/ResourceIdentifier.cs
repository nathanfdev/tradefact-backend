namespace FunctionApp.Integration.External.Common
{
    public class ResourceIdentifier : IReference
    {
        public string Id { get; set; }
        public ReferenceTypeId? TypeId { get; set; }
        public string Key { get; set; }
    }
}
