using System;

namespace WebAPI
{
    public class ToDoItem
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string[] Tags { get; set; }

        public DateTimeOffset DueDate { get; set; }
    }
}
