namespace Todo.Backend.TodoList.EventConsumer;

using System;
using System.Threading.Tasks;
using Contracts.Events.TodoList;
using Database.Cosmos;
using MassTransit;
using Microsoft.Extensions.Logging;

public class TodoListEventConsumer : IConsumer<TodoListCreatedEvent>, IConsumer<TodoListDeletedEvent>, IConsumer<TodoListUpdatedEvent>, IConsumer<TodoListItemCreatedEvent>
{
    private readonly ILogger<TodoListEventConsumer> _logger;
    private readonly CosmosDbContext _cosmosDbContext;
    public TodoListEventConsumer(ILogger<TodoListEventConsumer> logger, CosmosDbContext cosmosDbContext)
    {
        this._logger = logger;
        this._cosmosDbContext = cosmosDbContext;
    }

    public async Task Consume(ConsumeContext<TodoListCreatedEvent> context)
    {
        try
        {
            var @event = context.Message;
            var response = await this._cosmosDbContext.CreateItemAsync(@event);
        }
        catch (Exception exception)
        {
            this._logger.LogError(exception, "Failed to consume " + nameof(TodoListCreatedEvent));
        }
    }

    public async Task Consume(ConsumeContext<TodoListDeletedEvent> context)
    {
        try
        {
            var @event = context.Message;
            var response = await this._cosmosDbContext.CreateItemAsync(@event);
        }
        catch (Exception exception)
        {
            this._logger.LogError(exception, "Failed to consume " + nameof(TodoListDeletedEvent));
        }
    }

    public async Task Consume(ConsumeContext<TodoListUpdatedEvent> context)
    {
        try
        {
            var @event = context.Message;
            var response = await this._cosmosDbContext.CreateItemAsync(@event);
        }
        catch (Exception exception)
        {
            this._logger.LogError(exception, "Failed to consume " + nameof(TodoListUpdatedEvent));
        }
    }

    public async Task Consume(ConsumeContext<TodoListItemCreatedEvent> context)
    {
        try
        {
            var @event = context.Message;
            var response = await this._cosmosDbContext.CreateItemAsync(@event);
        }
        catch (Exception exception)
        {
            this._logger.LogError(exception, "Failed to consume " + nameof(TodoListItemCreatedEvent));
        }
    }
}