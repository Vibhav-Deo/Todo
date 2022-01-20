namespace Todo.Database.Models;

using System;
using Todo.Contracts.Enums;

public partial class TodoListItem
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public TodoListItemStatus Status { get; set; }
    public Guid TodoListId { get; set; }
    public virtual TodoList TodoList { get; set; }
}