using System;

namespace WebAPI
{
    public class ToDoItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string[] Tags { get; set; }

        public DateTimeOffset DueDate { get; set; }
        
        public bool IsDone { get; internal set; }
        
        public DateTimeOffset InsertedOn { get; internal set; }
    }
}
