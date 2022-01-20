namespace Todo.Backend.User.Services.RequestResponse;

using System;

public class GetUserByIdRequest
{
    public Guid UserId { get; set; }
}