using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Contracts.Enums;

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
