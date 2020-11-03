using System;
using System.Threading.Tasks;
using AutoFixture;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Service;
using static Service.ToDo;

namespace ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:23123");

            var client = new ToDoClient(channel);

            var fixture = CreateFixture();

            foreach (var item in fixture.CreateMany<ToDoItem>(10))
            {
                _ = await client.AddAsync(new ItemRequest
                {
                    Item = item
                });

                Console.WriteLine($"Added {item.Title}.");
                Console.WriteLine($"\tDue date on {item.DueDate}");
                Console.WriteLine();
            }
        }

        static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize<ToDoItem>(c => c
                .Without(p => p.Id)
                .Without(p => p.InsertedOn)
                .With(p => p.IsDone, false)
                .With(p => p.DueDate, (DateTimeOffset dto) => Timestamp.FromDateTimeOffset(dto))
                .Do(item => fixture.AddManyTo(item.Tags)));

            return fixture;
        }
    }
}
