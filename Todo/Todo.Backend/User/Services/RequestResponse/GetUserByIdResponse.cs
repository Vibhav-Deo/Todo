using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models = Todo.Database.Models;

namespace Todo.Backend.User.Services.RequestResponse
{
    public class GetUserByIdResponse
    {
        public Models.User User { get; set; }
    }
}
