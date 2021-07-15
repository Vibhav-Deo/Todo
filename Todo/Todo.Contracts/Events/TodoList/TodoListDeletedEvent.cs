using System;
using Todo.Contracts.Enums;

namespace Todo.Contracts.Events.TodoList
{
    public class TodoListDeletedEvent : IBaseEvent
    {
        public Guid EntityModifiedId { get; }
        public string Message { get; }
        public string EntityThatTookAction { get; }
        public DateTimeOffset CreatedOn { get; }

        public EntityType EntityType { get; }

        public TodoListDeletedEvent(Guid entityId, string entity, EntityType entityType,DateTimeOffset createdOn)
        {
            EntityModifiedId = entityId;
            EntityThatTookAction = entity;
            CreatedOn = createdOn;
            EntityType = entityType;
            Message = "Todo list deleted";
        }
    }
}
