using Models = Todo.Database.Models;
namespace Todo.Backend.User.Services
{
    public class GetUserByEmailResponse
    {
        public Models.User User { get; set; }
    }
}