using System;
using Core.Interfaces;
using Microsoft.Azure.Cosmos;

namespace Infrastructure.Data
{
    public class CosmosContainerProxy : ICosmosContainerProxy
    {
        private const string DATABASE_NAME = "TradeFact";

        private readonly Lazy<Container> _companies;

        private readonly Lazy<Container> _products;

        private readonly Lazy<Container> _importRates;

        private readonly Lazy<Container> _imports;

        private readonly Lazy<Container> _partner;

        private readonly Lazy<Container> _referenceData;

        private readonly Lazy<Container> _suppliers;

        public CosmosContainerProxy(CosmosClient client)
        {
            _partner = new Lazy<Container>(() => client.GetContainer(DATABASE_NAME, "Partners"));
            _companies = new Lazy<Container>(() => client.GetContainer(DATABASE_NAME, nameof(Companies)));
            _products = new Lazy<Container>(() => client.GetContainer(DATABASE_NAME, nameof(Products)));
            _imports = new Lazy<Container>(() => client.GetContainer(DATABASE_NAME, nameof(Imports)));
            _suppliers = new Lazy<Container>(() => client.GetContainer(DATABASE_NAME, nameof(Suppliers)));
            _importRates = new Lazy<Container>(() => client.GetContainer(DATABASE_NAME, nameof(ImportRates)));
            _referenceData = new Lazy<Container>(() => client.GetContainer(DATABASE_NAME, nameof(ReferenceData)));
        }

        public Container Companies => _companies.Value;
        public Container Products => _products.Value;

        public Container ImportRates => _importRates.Value;

        public Container Imports => _imports.Value;

        public Container Partner => _partner.Value;

        public Container ReferenceData => _referenceData.Value;

        public Container Suppliers => _suppliers.Value;
    }
}
