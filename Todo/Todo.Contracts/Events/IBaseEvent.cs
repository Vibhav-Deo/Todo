namespace Todo.Contracts.Events;

using System;
using MassTransit;

public interface IBaseEvent : CorrelatedBy<Guid>
{
}