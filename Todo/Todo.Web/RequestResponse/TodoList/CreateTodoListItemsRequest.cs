using System.Collections.Generic;

namespace Todo.Web.RequestResponse.TodoList
{
    public class CreateTodoListItemsRequest
    {
        public List<TodoListItemToBeCreated> TodoListItems { get; set; }
    }
    public class TodoListItemToBeCreated
    {
        public string Description { get; set; }
    }
}
