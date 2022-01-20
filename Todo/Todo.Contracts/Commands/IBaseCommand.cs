using System;
using MassTransit;

namespace Todo.Contracts.Commands
{
    public interface IBaseCommand : CorrelatedBy<Guid>
    {
    }
}
