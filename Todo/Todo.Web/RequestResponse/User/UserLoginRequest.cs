namespace Todo.Web.RequestResponse.User;

using System.ComponentModel.DataAnnotations;

public class UserLoginRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}