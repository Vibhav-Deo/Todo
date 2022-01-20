namespace Todo.Backend.User.EventConsumer;

using System;
using System.Threading.Tasks;
using global::Todo.Contracts.Events.User;
using global::Todo.Database.Cosmos;
using MassTransit;
using Microsoft.Extensions.Logging;

public class UserEventConsumer : IConsumer<UserCreatedEvent>, IConsumer<UserDeletedEvent>, IConsumer<UserUpdatedEvent>
{
    private readonly ILogger<UserEventConsumer> _logger;
    private readonly CosmosDbContext _cosmosDbContext;
    public UserEventConsumer(ILogger<UserEventConsumer> logger, CosmosDbContext cosmosDbContext)
    {
        _logger = logger;
        _cosmosDbContext = cosmosDbContext;
    }
    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        try
        {
            var @event = context.Message;
            var response = await _cosmosDbContext.CreateItemAsync(@event);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to consume " + nameof(UserCreatedEvent));
        }
    }

    public async Task Consume(ConsumeContext<UserDeletedEvent> context)
    {
        try
        {
            var @event = context.Message;
            var response = await _cosmosDbContext.CreateItemAsync(@event);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to consume " + nameof(UserDeletedEvent));
        }
    }

    public async Task Consume(ConsumeContext<UserUpdatedEvent> context)
    {
        try
        {
            var @event = context.Message;
            var response = await _cosmosDbContext.CreateItemAsync(@event);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to consume " + nameof(UserUpdatedEvent));
        }
    }
}