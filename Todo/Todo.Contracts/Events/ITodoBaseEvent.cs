using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Contracts.Events
{
    public interface ITodoBaseEvent
    {
        public Guid EntityId { get; }
        public string Message { get;}
        public string Entity { get;}
        public DateTimeOffset CreatedOn { get;}
    }
}
