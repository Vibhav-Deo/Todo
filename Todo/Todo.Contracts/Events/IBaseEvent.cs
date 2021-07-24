using MassTransit;
using System;
using Todo.Contracts.Enums;

namespace Todo.Contracts.Events
{
    public interface IBaseEvent : CorrelatedBy<Guid>
    {
    }
}
