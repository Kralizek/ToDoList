using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly ILogger<ToDoController> _logger;
        private readonly IToDoClient _todo;
        private readonly IMapper _mapper;

        public ToDoController (IToDoClient todo, IMapper mapper, ILogger<ToDoController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _todo = todo ?? throw new ArgumentNullException(nameof(todo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IReadOnlyList<ToDoItem>> Get()
        {
            _logger.LogInformation("Fetching all items");

            using var call = _todo.List(new Google.Protobuf.WellKnownTypes.Empty());

            var result = new List<ToDoItem>();

            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                result.Add(_mapper.Map<Service.ToDoItem, ToDoItem>(response));
            }

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ToDoItem> Get(Guid id)
        {
            _logger.LogInformation("Fetching item {ID}", id);

            var response = await _todo.GetAsync(new IdRequest { Id = id.ToString()  });

            return _mapper.Map<Service.ToDoItem, ToDoItem>(response);
        }

        [HttpPost]
        public async Task Post(ToDoItem item)
        {
            _logger.LogInformation("Adding new item");

            var itemToAdd = _mapper.Map<ToDoItem, Service.ToDoItem>(item);

            await _todo.AddAsync(new ItemRequest { Item = itemToAdd });
        }

        [HttpPost("{id}")]
        public async Task Post(Guid id, ToDoItem item)
        {
            _logger.LogInformation("Updating item {ID}", id);

            item.Id = id;

            var itemToUpdate = _mapper.Map<ToDoItem, Service.ToDoItem>(item);

            await _todo.UpdateAsync(itemToUpdate);
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            _logger.LogInformation("Deleting item {ID}", id);

            await _todo.RemoveAsync(new IdRequest { Id = id.ToString() });
        }
    }
}
