using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service;
using static Service.ToDo;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly ILogger<ToDoController> _logger;
        private readonly ToDoClient _todo;

        public ToDoController (ToDoClient todo, ILogger<ToDoController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _todo = todo ?? throw new ArgumentNullException(nameof(todo));
        }

        [HttpGet]
        public async Task<IReadOnlyList<ToDoItem>> Get()
        {
            using var call = _todo.List(new Google.Protobuf.WellKnownTypes.Empty());

            var result = new List<ToDoItem>();

            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                result.Add(new ToDoItem
                {
                    Id = Guid.Parse(response.Id),
                    Title = response.Title,
                    DueDate = response.DueDate.ToDateTimeOffset(),
                    IsDone = response.IsDone
                });
            }

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ToDoItem> Get(Guid id)
        {
            var response = await _todo.GetAsync(new IdRequest { Id = id.ToString()  });

            return new ToDoItem
            {
                Id = Guid.Parse(response.Id),
                Title = response.Title,
                DueDate = response.DueDate.ToDateTimeOffset(),
                IsDone = response.IsDone,
                Description = response.Description,
                Tags = response.Tags.ToArray(),
                InsertedOn = response.InsertedOn.ToDateTimeOffset()
            };
        }

        [HttpPost]
        public async Task Post(Service.ToDoItem item)
        {
            await _todo.AddAsync(new ItemRequest { Item = item });
        }
    }
}
