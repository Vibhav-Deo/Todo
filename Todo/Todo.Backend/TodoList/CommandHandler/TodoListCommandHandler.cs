namespace Todo.Backend.TodoList.CommandHandler;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Todo.Backend.TodoList.Repositories.Write;
using Todo.Contracts.Commands.TodoList;
using Todo.Contracts.Events.TodoList;
using Todo.Contracts.Exceptions;
using Todo.Contracts.StringResources;
using MassTransit;
using Microsoft.Extensions.Logging;

public class TodoListCommandHandler : IConsumer<CreateTodoListCommand>, IConsumer<UpdateTodoListCommand>, IConsumer<DeleteTodoListCommand>, IConsumer<CreateTodoListItemsCommand>
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

        _logger.LogInformation("Started consuming create todo list command");
        try
        {
            var todoListId = Guid.NewGuid();
            var todoList = new Database.Models.TodoList
            {
                Description = command.Description,
                Id = todoListId,
                IsDeleted = false,
                Name = command.Name,
                UserId = command.UserId
            };

            using (var transactionScope = new TransactionScope())
            {
                await _todoListWriteRepository.CreateTodoListAsync(todoList);
                transactionScope.Complete();
            }
            var correlationId = context.CorrelationId ?? Guid.NewGuid();
            await _bus.Publish(new TodoListCreatedEvent(todoListId.ToString(), "TodoList", Contracts.Enums.EntityType.TodoList, correlationId, DateTimeOffset.UtcNow));
            var createdTodoList = await _todoListWriteRepository.GetTodoListItems(todoListId);
            _logger.LogInformation("Todo list Created Successfully");
            await context.RespondAsync(createdTodoList);
        }
        catch (Exception exception)
        {
            _logger.LogError("Failed to create todo list");
            throw new TodoApplicationException(StringResources.FailedToCreateTodoList, exception);
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

    public async Task Consume(ConsumeContext<CreateTodoListItemsCommand> context)
    {
        var command = context.Message;

        try
        {
            _logger.LogError("Started consumption of create todo list items command");
            var todoListItems = new List<Database.Models.TodoListItem>();
            foreach (var item in command.TodoListItems)
            {
                todoListItems.Add(new Database.Models.TodoListItem { Id = Guid.NewGuid(), Description = item.Description, Status = Contracts.Enums.TodoListItemStatus.Created, TodoListId = command.TodoListId });
            }

            await _todoListWriteRepository.CreateTodoListItemsAsync(todoListItems);

            var correlationId = context.CorrelationId ?? Guid.NewGuid();
            var createdTodoListItems = await _todoListWriteRepository.GetTodoListItems(command.TodoListId);

            foreach (var listItems in createdTodoListItems.TodoListItems)
            {
                await _bus.Publish(new TodoListItemCreatedEvent(listItems.Id.ToString(), "TodoList", Contracts.Enums.EntityType.TodoListItem, correlationId, DateTimeOffset.UtcNow));
            }

            await context.RespondAsync(createdTodoListItems);
        }
        catch (Exception exception)
        {
            _logger.LogError("Failed to create todo list items");
            throw new TodoApplicationException(StringResources.FailedToCreateTodoListItems, exception);
        }
    }
}