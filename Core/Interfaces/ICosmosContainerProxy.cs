
using Microsoft.Azure.Cosmos;

namespace Core.Interfaces
{
    public interface ICosmosContainerProxy
    {
        public Container Companies { get; }

        public Container ImportRates { get; }

        public Container Imports { get; }

        public Container Partner { get; }
        public Container Products { get; }

        public Container ReferenceData { get; }

        public Container Suppliers { get; }
    }
}
