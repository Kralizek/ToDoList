using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Service
{

    public class ToDoService : ToDo.ToDoBase 
    {
        private readonly ILogger<ToDoService> _logger;
        private readonly IStorage _storage;

        public ToDoService (IStorage storage, ILogger<ToDoService> logger) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public override async Task<Result> Add(ItemRequest request, ServerCallContext context)
        {
            _ = request?.Item ?? throw new ArgumentNullException(nameof(request));

            _logger.LogDebug("Adding item");

            var newId = Guid.NewGuid();

            await _storage.AddAsync(newId, request.Item);

            return new Result { Response = Service.Result.Types.Response.Ok };
        }

        public override async Task<ToDoItem> Get(IdRequest request, ServerCallContext context)
        {
            if (Guid.TryParse(request.Id, out var guid))
            {
                return await _storage.GetAsync(guid);
            }

            throw new FormatException($"{request.Id} is not a valid Guid");            
        }

        public override async Task List(Empty request, IServerStreamWriter<ToDoItem> responseStream, ServerCallContext context)
        {
            await foreach (var item in _storage.GetAllAsync())
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task<Result> Update(ToDoItem request, ServerCallContext context)
        {
            if (Guid.TryParse(request.Id, out var guid))
            {
                await _storage.UpdateAsync(guid, request);

                return new Result { Response = Result.Types.Response.Ok };
            }

            return new Result { Response = Result.Types.Response.Error };
        }

        public override async Task<Result> Remove(IdRequest request, ServerCallContext context)
        {
            if (Guid.TryParse(request.Id, out var guid))
            {
                await _storage.RemoveAsync(guid);

                return new Result { Response = Result.Types.Response.Ok };
            }

            return new Result { Response = Result.Types.Response.Error };
        }
    }
}