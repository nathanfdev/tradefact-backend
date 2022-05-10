using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.Azure.Cosmos;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Core.Interfaces
{
    public interface ICosmosRepository
    {
        Task<T[]> CreateAsync<T>(IEnumerable<T> items, Container container) where T : CosmosItem<T>;

        ValueTask<T> CreateAsync<T>(T item, Container container) where T : CosmosItem<T>;

        ValueTask<T> DeactivateAsync<T>(string id, string partitionKeyValue, Container container)
            where T : CosmosItem<T>;

        ValueTask<T> DestroyAsync<T>(string id, string partitionKeyValue, Container container) where T : CosmosItem<T>;

        ValueTask<T> GetAsync<T>(string id, string partitionKeyValue, Container container) where T : CosmosItem<T>;

        ValueTask<IEnumerable<T>> GetAsync<T>(
        Expression<Func<T, bool>> predicate, Container container, int maxItemCount = 100)
            where T : CosmosItem<T>;

        ValueTask<IEnumerable<T>> GetAsync<T>(QueryDefinition query, Container container, int maxItemCount = 100)
            where T : CosmosItem<T>;

        ValueTask<T> ReactivateAsync<T>(string id, string partitionKeyValue, Container container)
            where T : CosmosItem<T>;

        ValueTask<T> UpdateAsync<T>(T item, Container container) where T : CosmosItem<T>;
    }
}