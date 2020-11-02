using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
            using var call = _todo.ListItems(new ListRequest());

            var result = new List<ToDoItem>();

            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                result.Add(new ToDoItem
                {
                    Title = response.Title,
                    Description = response.Description,
                    DueDate = response.DueDate.ToDateTimeOffset(),
                    Tags = response.Tags.ToArray()
                });
            }

            return result;
        }

        [HttpPost]
        public async Task Post(Service.ToDoItem item)
        {
            await _todo.AddItemAsync(new AddRequest{ Item = item });
        }
    }

    [ApiController, Route("[controller]")]
    public class DebugController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get([FromServices] IConfiguration configuration)
        {
            return configuration.AsEnumerable().Select(item => $"{item.Key}={item.Value}");
        }
    }
}
