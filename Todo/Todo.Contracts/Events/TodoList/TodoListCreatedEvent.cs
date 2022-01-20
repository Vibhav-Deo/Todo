namespace Todo.Contracts.Events.TodoList;

using System;
using Todo.Contracts.Enums;

public class TodoListCreatedEvent : IBaseEvent
{
    public string Id { get; set; }
    public string Message { get; set; }
    public string EntityThatTookAction { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public EntityType EntityType { get; set; }
    public Guid CorrelationId { get; set; }

    public TodoListCreatedEvent(string entityId, string entity, EntityType entityType, Guid correlationId, DateTimeOffset createdOn)
    {
        Id = entityId;
        EntityThatTookAction = entity;
        CreatedOn = createdOn;
        EntityType = entityType;
        CorrelationId = correlationId;
        Message = "Todo list created";
    }
}