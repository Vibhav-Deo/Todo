using MassTransit;
using System;
using System.Threading.Tasks;
using Todo.Contracts.Commands;
using Todo.Contracts.Commands.TodoList;

namespace Todo.Backend.TodoList.CommandHandler
{
    public class TodoListCommandHandler : IConsumer<CreateTodoListCommand>, IConsumer<UpdateTodoListCommand>, IConsumer<DeleteTodoListCommand>
    {
        public Task Consume(ConsumeContext<CreateTodoListCommand> context)
        {
            return Task.FromResult("ALl good");
        }

        public Task Consume(ConsumeContext<UpdateTodoListCommand> context)
        {
            throw new NotImplementedException();
        }

        public Task Consume(ConsumeContext<DeleteTodoListCommand> context)
        {
            throw new NotImplementedException();
        }
    }
}
