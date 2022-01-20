namespace Todo.Web.RequestResponse.TodoList;

using System.Collections.Generic;

public class CreateTodoListItemsRequest
{
    public List<TodoListItemToBeCreated> TodoListItems { get; set; }
}
public class TodoListItemToBeCreated
{
    public string Description { get; set; }
}