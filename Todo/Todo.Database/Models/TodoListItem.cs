using System;
using Todo.Contracts.Enums;

namespace Todo.Database.Models
{
    public partial class TodoListItem
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public TodoListItemStatus Status { get; set; }
    }
}
