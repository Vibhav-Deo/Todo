using System;
using Todo.Contracts.Enums;
using Todo.Contracts.Events.TodoList;

namespace Todo.Contracts.Events.TodoList
{
    public class TodoListUpdatedEvent : IBaseEvent
    {
        public Guid EntityModifiedId { get; }
        public string Message { get; }
        public string EntityThatTookAction { get; }
        public DateTimeOffset CreatedOn { get; }

        public EntityType EntityType { get; }

        public TodoListUpdatedEvent(Guid entityId, string entity, EntityType entityType,DateTimeOffset createdOn)
        {
            EntityModifiedId = entityId;
            EntityThatTookAction = entity;
            CreatedOn = createdOn;
            EntityType = entityType;
            Message = "Todo list updated";
        }
    }
}
