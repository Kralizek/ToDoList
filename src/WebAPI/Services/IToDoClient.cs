namespace WebAPI.Services
{
    using System;
    using System.Threading;
    using Service;
    using Grpc.Core;
    using Google.Protobuf.WellKnownTypes;

    public interface IToDoClient
    {
        AsyncUnaryCall<Result> AddAsync(ItemRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);

        AsyncServerStreamingCall<ToDoItem> List(Empty request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);

        AsyncUnaryCall<ToDoItem> GetAsync(IdRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);

        AsyncUnaryCall<Result> RemoveAsync(IdRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);

        AsyncUnaryCall<Result> UpdateAsync(ToDoItem request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default);
    }
}

namespace Service
{
    using WebAPI.Services;

    public static partial class ToDo
    {
        public partial class ToDoClient : IToDoClient
        {

        }
    }
}