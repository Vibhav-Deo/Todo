using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Contracts.Commands.TodoList;
using Todo.Contracts.Commands.User;
using Todo.Database.Models;
using Todo.Web.RequestResponse.TodoList;
using Todo.Web.RequestResponse.User;

namespace Todo.Web.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<UserRegisterationRequest, CreateUserCommand>();
            CreateMap<User, UserResponse>();
            CreateMap<CreateTodoListRequest, CreateTodoListCommand>();
        }
    }
}
