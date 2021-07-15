using MassTransit;
using System.Threading.Tasks;
using Todo.Contracts.Events.TodoList;

namespace Todo.Backend.Todo.EventConsumers
{
    public class TodoListEventConsumer : IConsumer<TodoListCreatedEvent>, IConsumer<TodoListDeletedEvent>, IConsumer<TodoListUpdatedEvent>
    {
        public Task Consume(ConsumeContext<TodoListCreatedEvent> context)
        {
            return Task.FromResult("ALl good");
        }

        public Task Consume(ConsumeContext<TodoListDeletedEvent> context)
        {
            throw new System.NotImplementedException();
        }

        public Task Consume(ConsumeContext<TodoListUpdatedEvent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
