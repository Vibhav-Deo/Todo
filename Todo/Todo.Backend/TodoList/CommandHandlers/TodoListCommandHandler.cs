using MassTransit;
using System;
using System.Threading.Tasks;
using Todo.Backend.TodoList.CommandHandlers.Commands;

namespace Todo.Backend.TodoList.CommandHandlers
{
    public class TodoListCommandHandler : IConsumer<TodoListCommand>
    {
        public Task Consume(ConsumeContext<TodoListCommand> context)
        {
            throw new NotImplementedException();
        }
    }
}
