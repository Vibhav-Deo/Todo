namespace Todo.Backend.TodoList.Services;

using System;
using System.Threading.Tasks;
using global::Todo.Backend.TodoList.Repositories.Read;
using Models = global::Todo.Database.Models;

public interface ITodoListReadService
{
    Task<Models.TodoList> GetTodoListByIdAsync(Guid todolistId);
}
public class TodoListReadService : ITodoListReadService
{
    private readonly ITodoListReadRepository _todoListReadRepository;
    public TodoListReadService(ITodoListReadRepository todoListReadRepository)
    {
        _todoListReadRepository = todoListReadRepository;
    }
    public async Task<Models.TodoList> GetTodoListByIdAsync(Guid todolistId)
    {
        var todoList = await _todoListReadRepository.GetTodoListByIdAsync(todolistId);
        return todoList;
    }
}