using Ardalis.GuardClauses;
using Core.Interfaces;
using Core.Models;
using Mapster;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Infrastructure.Data
{
    public class CosmosRepository : ICosmosRepository
    {
        public async ValueTask<T> CreateAsync<T>(T item, Container container)
            where T : CosmosItem<T>
        {
            Guard.Against.Null(item, nameof(item));
            Guard.Against.NullOrEmpty(item.PartitionKeyValue, nameof(item.PartitionKeyValue));
            Guard.Against.Null(container, nameof(container));

            item.Id = Guid.NewGuid();
            item.IsActive = true;
            item.UpsertDate = DateTime.UtcNow;

            var response = await container.CreateItemAsync(item, new PartitionKey(item.PartitionKeyValue.ToString())).ConfigureAwait(false);

            return response.Resource;
        }

        public Task<T[]> CreateAsync<T>(IEnumerable<T> values, Container container) where T : CosmosItem<T> => Task.WhenAll(values.Select(v => CreateAsync(v, container).AsTask()));

        public async ValueTask<T> DeactivateAsync<T>(string id, string partitionKeyValue, Container container)
            where T : CosmosItem<T>
        {
            Guard.Against.NullOrEmpty(id, nameof(id));
            Guard.Against.NullOrEmpty(partitionKeyValue, nameof(partitionKeyValue));
            Guard.Against.Null(container, nameof(container));

            var item = await GetAsync<T>(id, partitionKeyValue, container).ConfigureAwait(false);

            item.UpsertDate = DateTime.UtcNow;
            item.IsActive = false;

            return await UpdateAsync(item, container).ConfigureAwait(false);
        }

        public async ValueTask<T> DestroyAsync<T>(string id, string partitionKeyValue, Container container)
            where T : CosmosItem<T>
        {
            Guard.Against.NullOrEmpty(id, nameof(id));
            Guard.Against.NullOrEmpty(partitionKeyValue, nameof(partitionKeyValue));
            Guard.Against.Null(container, nameof(container));

            var response = await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKeyValue)).ConfigureAwait(false);

            return response.Resource;
        }

        public async ValueTask<T> GetAsync<T>(string id, string partitionKeyValue, Container container)
            where T : CosmosItem<T>
        {
            Guard.Against.NullOrEmpty(id, nameof(id));
            Guard.Against.NullOrEmpty(partitionKeyValue, nameof(partitionKeyValue));
            Guard.Against.Null(container, nameof(container));

            var response = await container.ReadItemAsync<T>(id, new PartitionKey(partitionKeyValue)).ConfigureAwait(false);

            return response.Resource;
        }

        public async ValueTask<IEnumerable<T>> GetAsync<T>(Expression<Func<T, bool>> predicate, Container container, int maxItemCount = 100)
            where T : CosmosItem<T>
        {
            Guard.Against.Null(predicate, nameof(predicate));
            Guard.Against.Null(container, nameof(container));

            var iterator = container
                .GetItemLinqQueryable<T>(requestOptions: new QueryRequestOptions { MaxItemCount = maxItemCount })
                .Where(predicate)
                .ToFeedIterator();

            var results = new List<T>();
            while(iterator.HasMoreResults)
            {
                foreach(var result in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    results.Add(result);
                }
            }

            return results;
        }

        public async ValueTask<IEnumerable<T>> GetAsync<T>(QueryDefinition query, Container container, int maxItemCount = 100)
            where T : CosmosItem<T>
        {
            Guard.Against.Null(query, nameof(query));
            Guard.Against.Null(container, nameof(container));

            var iterator =
                    container
                .GetItemQueryIterator<T>(query, requestOptions: new QueryRequestOptions { MaxItemCount = maxItemCount });

            var results = new List<T>();

            while(iterator.HasMoreResults)
            {
                foreach(var result in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    results.Add(result);
                }
            }

            return results;
        }

        public async ValueTask<T> ReactivateAsync<T>(string id, string partitionKeyValue, Container container)
            where T : CosmosItem<T>
        {
            Guard.Against.NullOrEmpty(id, nameof(id));
            Guard.Against.NullOrEmpty(partitionKeyValue, nameof(partitionKeyValue));
            Guard.Against.Null(container, nameof(container));

            var item = await GetAsync<T>(id, partitionKeyValue, container).ConfigureAwait(false);

            item.UpsertDate = DateTime.UtcNow;
            item.IsActive = true;

            return await UpdateAsync(item, container).ConfigureAwait(false);
        }

        public async ValueTask<T> UpdateAsync<T>(T item, Container container)
            where T : CosmosItem<T>
        {
            Guard.Against.Null(item, nameof(item));
            Guard.Against.NullOrEmpty(item.PartitionKeyValue, nameof(item.PartitionKeyValue));
            Guard.Against.NullOrEmpty(item.ETag, nameof(item.ETag));
            Guard.Against.Null(container, nameof(container));

            item.UpsertDate = DateTime.UtcNow;

            var itemRequestOptions = new ItemRequestOptions { IfMatchEtag = item.ETag };

            var updatedItem = await GetAsync<T>(item.Id.ToString(), item.PartitionKeyValue, container).ConfigureAwait(false);

            updatedItem = item.Adapt(updatedItem);
            updatedItem.UpsertDate = DateTime.UtcNow;

            var response = await container.UpsertItemAsync<T>(updatedItem, new PartitionKey(updatedItem.PartitionKeyValue), itemRequestOptions).ConfigureAwait(false);

            return response.Resource;
        }
    }
}