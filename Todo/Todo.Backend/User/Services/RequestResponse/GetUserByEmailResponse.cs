namespace Todo.Backend.User.Services
{
    using Models = global::Todo.Database.Models;

    public class GetUserByEmailResponse
    {
        public Models.User User { get; set; }
    }
}
