using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Contracts.Commands
{
    public interface IHandle<in TCommand> : IConsumer where TCommand : class
    {
        Task Handler(ConsumeContext<TCommand> context);
    }
}
