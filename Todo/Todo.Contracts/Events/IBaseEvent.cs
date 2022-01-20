using System;
using MassTransit;

namespace Todo.Contracts.Events
{
    public interface IBaseEvent : CorrelatedBy<Guid>
    {
    }
}
