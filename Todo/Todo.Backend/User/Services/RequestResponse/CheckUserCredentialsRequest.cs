namespace Todo.Backend.User.Services.RequestResponse;

public class CheckUserCredentialsRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}