namespace Todo.Contracts.Events.User;

using System;
using Todo.Contracts.Enums;

public class UserUpdatedEvent : IBaseEvent
{
    public string Id { get; }
    public string Message { get; }
    public string EntityThatTookAction { get; }
    public DateTimeOffset CreatedOn { get; }
    public EntityType EntityType { get; }
    public Guid CorrelationId { get; set; }

    public UserUpdatedEvent(string entityId, string entity, EntityType entityType, Guid correlationId, DateTimeOffset createdOn)
    {
        Id = entityId;
        EntityThatTookAction = entity;
        CreatedOn = createdOn;
        EntityType = entityType;
        CorrelationId = correlationId;
        Message = "User account updated";
    }
}