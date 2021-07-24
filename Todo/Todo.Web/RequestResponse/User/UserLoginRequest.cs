using System.ComponentModel.DataAnnotations;

namespace Todo.Web.RequestResponse.User
{
    public class UserLoginRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
