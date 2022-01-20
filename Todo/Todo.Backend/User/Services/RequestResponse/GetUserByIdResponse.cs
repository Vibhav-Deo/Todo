using Models = Todo.Database.Models;

namespace Todo.Backend.User.Services.RequestResponse
{
    public class GetUserByIdResponse
    {
        public Models.User User { get; set; }
    }
}
