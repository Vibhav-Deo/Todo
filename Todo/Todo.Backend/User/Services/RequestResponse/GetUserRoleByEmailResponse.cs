using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Contracts.Enums;

namespace Todo.Backend.User.Services.RequestResponse
{
    public class GetUserRoleByEmailResponse
    {
        public UserRoles Role { get; set; }
    }
}
