using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace Service
{
    public class DynamoDBTableStorage : IStorage
    {
        private readonly IDynamoDBContext _context;

        public DynamoDBTableStorage(IDynamoDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task AddAsync(Guid guid, ToDoItem item)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ToDoItem> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ToDoItem> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guid guid, ToDoItem item)
        {
            throw new NotImplementedException();
        }
    }
}