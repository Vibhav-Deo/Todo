namespace Todo.Contracts.Commands.TodoList;

using System;
using System.Collections.Generic;

public class CreateTodoListItemsCommand : IBaseCommand
{
    public List<TodoListItemToBeCreated> TodoListItems { get; set; }
    public Guid TodoListId { get; set; }
    public Guid CorrelationId => Guid.NewGuid();
}
public class TodoListItemToBeCreated
{
    public string Description { get; set; }
}