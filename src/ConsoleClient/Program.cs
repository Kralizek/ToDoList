using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Service;
using static Service.ToDo;
using static Service.ToDoItem.Types;

namespace ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            var client = new ToDoClient(channel);

            var item = new ToDoItem
            {
                Title = "My first item",
                Description = "Hello world",
                DueDate = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow.AddDays(7)),
                Priority = Priority.Highest
            };

            item.Tags.Add("No idea");

            await client.AddItemAsync(new AddRequest
            {
                Item = item
            });

            Console.WriteLine("Hello World!");
        }
    }
}
