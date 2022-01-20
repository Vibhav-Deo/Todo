namespace Todo.Contracts.Commands;

using System;
using MassTransit;

public interface IBaseCommand : CorrelatedBy<Guid>
{
}