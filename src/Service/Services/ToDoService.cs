using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class ToDoService : ToDo.ToDoBase 
    {
        private readonly ILogger<ToDoService> _logger;
        private readonly IList<ToDoItem> _items = new List<ToDoItem>();

        public ToDoService (ILogger<ToDoService> logger) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<AddReply> AddItem(AddRequest request, ServerCallContext context)
        {
            _ = request?.Item ?? throw new ArgumentNullException(nameof(request));

            _logger.LogDebug("Adding item");

            _items.Add(request.Item);

            return new AddReply { Response = Response.Ok };
        }

        public override async Task ListItems(ListRequest request, IServerStreamWriter<ToDoItem> responseStream, ServerCallContext context)
        {
            foreach (var item in _items)
            {
                await responseStream.WriteAsync(item);
            }
        }
    }
}