using System.Collections.Generic;
using Todo.Database.Models;

namespace Todo.Backend.TodoList.Repositories.Dtos
{
    public class CreatedTodoListItemsDto
    {
        public IList<TodoListItem> TodoListItems { get; set; }
    }
}
