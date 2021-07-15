using System;
using Todo.Contracts.Enums;

namespace Todo.Contracts.Events
{
    public interface IBaseEvent
    {
        public Guid EntityModifiedId { get; }
        public string Message { get;}
        public string EntityThatTookAction { get;}
        public DateTimeOffset CreatedOn { get;}
        public EntityType EntityType { get; }
    }
}
