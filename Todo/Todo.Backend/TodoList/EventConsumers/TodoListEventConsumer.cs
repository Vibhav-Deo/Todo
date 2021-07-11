using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Contracts.Events;

namespace Todo.Backend.Todo.EventConsumers
{
    public class TodoListEventConsumer : IConsumer<TodoListCreatedEvent>
    {
        public Task Consume(ConsumeContext<TodoListCreatedEvent> context)
        {
            return Task.FromResult("ALl good");
        }
    }
}
