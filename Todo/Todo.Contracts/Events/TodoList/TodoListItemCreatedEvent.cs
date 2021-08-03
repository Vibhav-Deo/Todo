﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Contracts.Enums;

namespace Todo.Contracts.Events.TodoList
{
    public class TodoListItemCreatedEvent : IBaseEvent
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string EntityThatTookAction { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public EntityType EntityType { get; set; }
        public Guid CorrelationId { get; set; }

        public TodoListItemCreatedEvent(string entityId, string entity, EntityType entityType, Guid correlationId, DateTimeOffset createdOn)
        {
            Id = entityId;
            EntityThatTookAction = entity;
            CreatedOn = createdOn;
            EntityType = entityType;
            CorrelationId = correlationId;
            Message = "Todo list item created";
        }
    }
}