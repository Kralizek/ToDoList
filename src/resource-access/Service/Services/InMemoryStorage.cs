using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace Service
{
    public class InMemoryStorage : IStorage
    {
        private readonly ConcurrentDictionary<Guid, ToDoItem> _items = new ConcurrentDictionary<Guid, ToDoItem>();

        public Task AddAsync(Guid guid, ToDoItem item)
        {
            item.Id = guid.ToString();
            item.InsertedOn = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow);
            
            _items.AddOrUpdate(guid, id => item, (id, old) => item);

            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<ToDoItem> GetAllAsync()
        {
            foreach (var item in _items)
            {
                yield return item.Value;
            }
        }

        public Task<ToDoItem> GetAsync(Guid id)
        {
            if (_items.TryGetValue(id, out var item))
            {
                return Task.FromResult(item);
            }

            return null;
        }

        public Task RemoveAsync(Guid id)
        {
            _items.TryRemove(id, out _);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(Guid guid, ToDoItem item)
        {
            _items.AddOrUpdate(guid, id => item, (id, old) => item);

            return Task.CompletedTask;
        }
    }
}