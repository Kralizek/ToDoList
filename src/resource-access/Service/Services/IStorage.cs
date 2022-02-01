using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public interface IStorage
    {
        Task AddAsync(Guid guid, ToDoItem item);

        IAsyncEnumerable<ToDoItem> GetAllAsync();

        Task<ToDoItem> GetAsync(Guid id);

        Task RemoveAsync(Guid id);

        Task UpdateAsync(Guid guid, ToDoItem item);
    }
}