namespace Todo.Backend.User.Repositories.Write;

using System;
using System.Linq;
using System.Threading.Tasks;
using global::Todo.Contracts.Enums;
using global::Todo.Database.Models;
using Models = global::Todo.Database.Models;

public interface IUserWriteRepository
{
    Task RegisterUserAsync(Models.User user);
    Task<Models.User> GetUserAsync(Guid userId);
    Task<Models.User> GetUserByUserNameAsync(string userName);
    Task<Models.User> GetUserByEmailAsync(string email);
    Task<UserRoles> GetUserRoleByEmailAsync(string email);
    Task<bool> IsExistingUserAsync(string email);
    Task<bool> IsExistingUserAsync(Guid userId);
}
public class UserWriteRepository : IUserWriteRepository
{
    private readonly TodoListContext _context;

    public UserWriteRepository(TodoListContext context)
    {
        _context = context;
    }

    public Task<UserRoles> GetUserRoleByEmailAsync(string email)
    {
        return Task.FromResult((UserRoles)_context.Users.FirstOrDefault(user => user.Email == email).Role);
    }

    public Task<bool> IsExistingUserAsync(string email)
    {
        return Task.FromResult(_context.Users.FirstOrDefault(user => user.Email == email) != null);
    }

    public Task<bool> IsExistingUserAsync(Guid userId)
    {
        return Task.FromResult(_context.Users.FirstOrDefault(user => user.Id == userId) != null);
    }

    public Task<Models.User> GetUserByUserNameAsync(string userName)
    {
        return Task.FromResult(_context.Users.FirstOrDefault(user => user.UserName == userName));
    }

    public Task<Models.User> GetUserAsync(Guid userId)
    {
        return Task.FromResult(_context.Users.FirstOrDefault(user => user.Id == userId));
    }

    public Task<Models.User> GetUserByEmailAsync(string email)
    {
        return Task.FromResult(_context.Users.FirstOrDefault(user => user.Email == email));
    }

    public Task RegisterUserAsync(Models.User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return Task.CompletedTask;
    }
}