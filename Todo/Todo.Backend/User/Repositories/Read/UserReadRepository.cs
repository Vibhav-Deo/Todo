using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Contracts.Enums;
using Todo.Database.Models;
using Models =  Todo.Database.Models;

namespace Todo.Backend.User.Repository
{
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
    }
}
