using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Web.RequestResponse.TodoList
{
    public class CreateTodoListRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
