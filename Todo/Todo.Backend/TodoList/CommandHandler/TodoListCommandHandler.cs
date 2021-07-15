using MassTransit;
using System;
using System.Threading.Tasks;
using Todo.Contracts.Commands;

namespace Todo.Backend.TodoList.CommandHandlers
{
    public class TodoListCommandHandler : IConsumer<TodoListCommand>
    {
        public Task Consume(ConsumeContext<TodoListCommand> context)
        {
            return Task.FromResult("ALl good");
        }
    }
}
