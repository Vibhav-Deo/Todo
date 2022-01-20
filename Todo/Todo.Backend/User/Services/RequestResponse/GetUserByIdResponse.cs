namespace Todo.Backend.User.Services.RequestResponse;

using Models = global::Todo.Database.Models;

public class GetUserByIdResponse
{
    public Models.User User { get; set; }
}