using System;
using System.Collections.Generic;

namespace Todo.Contracts.Commands.TodoList
{
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
}
