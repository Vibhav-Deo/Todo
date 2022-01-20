namespace Todo.Backend.TodoList.Repositories.Write;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using global::Todo.Backend.TodoList.Repositories.Dtos;
using global::Todo.Database.Models;
using Microsoft.EntityFrameworkCore;
using Models = global::Todo.Database.Models;

public interface ITodoListWriteRepository
{
    Task CreateTodoListAsync(Models.TodoList todoList);
    Task<Models.TodoList> GetTodoListByIdAsync(Guid todoListId);
    Task CreateTodoListItemsAsync(List<Models.TodoListItem> todoListItem);
    Task<CreatedTodoListItemsDto> GetTodoListItems(Guid todoListId);
}

public class TodoListWriteRepository : ITodoListWriteRepository
{
    private readonly TodoListContext _context;

    public TodoListWriteRepository(TodoListContext context)
    {
        _context = context;
    }
    public Task CreateTodoListAsync(Models.TodoList todoList)
    {
        _context.TodoLists.Add(todoList);
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    public Task CreateTodoListItemsAsync(List<TodoListItem> todoListItem)
    {
        _context.TodoListItems.AddRange(todoListItem);
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    public Task<Models.TodoList> GetTodoListByIdAsync(Guid todoListId)
    {
        return Task.FromResult(_context.TodoLists.Include(list => list.User).Single(list => list.Id == todoListId));
    }

    public Task<CreatedTodoListItemsDto> GetTodoListItems(Guid todoListId)
    {
        var createdTodoListItems = new CreatedTodoListItemsDto
        {
            TodoListItems = _context.TodoListItems.Include(list => list.TodoList).Include(list => list.TodoList.User).Where(item => item.TodoListId == todoListId).ToList()
        };
        return Task.FromResult(createdTodoListItems);
    }
}