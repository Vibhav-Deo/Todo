using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Contracts.Events
{
    public class TodoListCreatedEvent: ITodoBaseEvent
    {
        public Guid EntityId { get;}
        public string Message { get;}
        public string Entity { get;}
        public DateTimeOffset CreatedOn {get; }

        public TodoListCreatedEvent(Guid entityId, string entity, DateTimeOffset createdOn)
        {
            EntityId = entityId;
            Entity = entity;
            CreatedOn = createdOn;
            Message = "Todo list created";
        }
        public TodoListCreatedEvent()
        {

        }
    }
}
