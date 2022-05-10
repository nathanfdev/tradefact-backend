using System;

namespace FunctionApp.Integration.External.Common
{
    public class Resource<T> : IReferenceable<T>, IVersioned<T>, IIdReferencable<T> where T : Resource<T>
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }

        public IReference<T> ToReference()
        {
            return new Reference<T>() { Id = this.Id, Obj = this as T };
        }
    }
}
