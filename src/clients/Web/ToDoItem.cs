using System;
using System.ComponentModel.DataAnnotations;

namespace Web
{
    public class ToDoItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string[] Tags { get; set; }

        [DataType(DataType.DateTime)]
        public DateTimeOffset DueDate { get; set; } = DateTimeOffset.UtcNow.AddDays(7);
        
        public bool IsDone { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTimeOffset InsertedOn { get; set; }

        public Priority Priority { get; set; } = Priority.Medium;
    }

    public enum Priority
    {
        Lowest = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Highest = 4
    }
}
