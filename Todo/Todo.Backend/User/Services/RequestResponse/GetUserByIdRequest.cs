using System;

namespace Todo.Backend.User.Services.RequestResponse
{
    public class GetUserByIdRequest
    {
        public Guid UserId { get; set; }
    }
}
