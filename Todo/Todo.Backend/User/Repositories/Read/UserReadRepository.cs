namespace Todo.Backend.User.Repositories.Read;

using System;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Enums;
using Database.Models;
using Models = global::Todo.Database.Models;

public interface IUserReadRepository
{
    Task<Models.User> GetUserAsync(Guid userId);
    Task<Models.User> GetUserByUserNameAsync(string userName);
    Task<Models.User> GetUserByEmailAsync(string email);
    Task<UserRoles> GetUserRoleByEmailAsync(string email);
    Task<bool> IsExistingUserAsync(string email);
    Task<bool> IsExistingUserAsync(Guid userId);
}
public class UserReadRepository : IUserReadRepository
{
    private readonly TodoListContext _context;

    public UserReadRepository(TodoListContext context)
    {
        this._context = context;
    }
    public Task<UserRoles> GetUserRoleByEmailAsync(string email)
    {
        return Task.FromResult((UserRoles)this._context.Users.FirstOrDefault(user => user.Email == email).Role);
    }

    public Task<bool> IsExistingUserAsync(string email)
    {
        return Task.FromResult(this._context.Users.FirstOrDefault(user => user.Email == email) != null);
    }

    public Task<bool> IsExistingUserAsync(Guid userId)
    {
        return Task.FromResult(this._context.Users.FirstOrDefault(user => user.Id == userId) != null);
    }

    public Task<Models.User> GetUserByUserNameAsync(string userName)
    {
        return Task.FromResult(this._context.Users.FirstOrDefault(user => user.UserName == userName));
    }

    public Task<Models.User> GetUserAsync(Guid userId)
    {
        return Task.FromResult(this._context.Users.FirstOrDefault(user => user.Id == userId));
    }

    public Task<Models.User> GetUserByEmailAsync(string email)
    {
        return Task.FromResult(this._context.Users.FirstOrDefault(user => user.Email == email));
    }
}