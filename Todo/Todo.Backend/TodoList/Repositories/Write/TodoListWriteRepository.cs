using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Database.Models;
using Models = Todo.Database.Models;

namespace Todo.Backend.TodoList.Repositories.Write
{
    public interface ITodoListWriteRepository
    {
        Task CreateTodoList(Models.TodoList todoList);
    }

    public class TodoListWriteRepository : ITodoListWriteRepository
    {
        private readonly TodoListContext _context;

        public TodoListWriteRepository(TodoListContext context)
        {
            _context = context;
        }
        public Task CreateTodoList(Models.TodoList todoList)
        {
            _context.TodoLists.Add(todoList);
            _context.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
