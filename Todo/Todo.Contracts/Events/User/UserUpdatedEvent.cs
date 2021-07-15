using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Contracts.Enums;

namespace Todo.Contracts.Events.User
{
    public class UserUpdatedEvent : IBaseEvent
    {
        public Guid EntityModifiedId { get; }
        public string Message { get; }
        public string EntityThatTookAction { get; }
        public DateTimeOffset CreatedOn { get; }
        public EntityType EntityType { get; }

        public UserUpdatedEvent(Guid entityId, string entity, EntityType entityType, DateTimeOffset createdOn)
        {
            EntityModifiedId = entityId;
            EntityThatTookAction = entity;
            CreatedOn = createdOn;
            EntityType = entityType;
            Message = "User list updated";
        }
    }
}
