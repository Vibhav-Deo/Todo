namespace Todo.Backend.TodoList.Repositories.Dtos;

using System.Collections.Generic;
using global::Todo.Database.Models;

public class CreatedTodoListItemsDto
{
    public IList<TodoListItem> TodoListItems { get; set; }
}