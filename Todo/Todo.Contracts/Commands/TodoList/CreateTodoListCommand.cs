namespace Todo.Contracts.Commands.TodoList;

using System;

public class CreateTodoListCommand
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public string Description { get; set; }
}