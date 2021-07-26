using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Transactions;
using Todo.Backend.TodoList.Repositories.Write;
using Todo.Contracts.Commands;
using Todo.Contracts.Commands.TodoList;
using Todo.Contracts.Events.TodoList;
using Todo.Contracts.Exceptions;

namespace Todo.Backend.TodoList.CommandHandler
{
    public class TodoListCommandHandler : IConsumer<CreateTodoListCommand>, IConsumer<UpdateTodoListCommand>, IConsumer<DeleteTodoListCommand>
    {
        private readonly ILogger<TodoListCommandHandler> _logger;
        private readonly ITodoListWriteRepository _todoListWriteRepository;
        private readonly IBus _bus;
        public TodoListCommandHandler(ITodoListWriteRepository todoListWriteRepository, ILogger<TodoListCommandHandler> logger, IBus bus)
        {
            _logger = logger;
            _todoListWriteRepository = todoListWriteRepository;
            _bus = bus;
        }
        public async Task Consume(ConsumeContext<CreateTodoListCommand> context)
        {
            var command = context.Message;
            var todoListId = Guid.NewGuid();
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    await _todoListWriteRepository.CreateTodoList(new Database.Models.TodoList
                    {
                        Description = command.Description,
                        Id = todoListId,
                        IsDeleted = false,
                        Name = command.Name,
                        UserId = command.UserId
                    });
                    transactionScope.Complete();
                }
                var correlationId = context.CorrelationId ?? Guid.NewGuid();
                await _bus.Publish(new TodoListCreatedEvent(todoListId.ToString(), "TodoList", Contracts.Enums.EntityType.TodoList, correlationId, DateTimeOffset.UtcNow));
            }
            catch (Exception exception)
            {
                throw new TodoApplicationException("Failed to create todo list", exception);
            }
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
