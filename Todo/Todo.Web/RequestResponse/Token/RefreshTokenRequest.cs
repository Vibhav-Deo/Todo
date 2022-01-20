using System.ComponentModel.DataAnnotations;

namespace Todo.Web.RequestResponse.Token
{
    public class RefreshTokenRequest
    {

        [Required]
        public string RefreshToken { get; set; }
    }
}
