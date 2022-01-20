namespace Todo.Web.RequestResponse.Token;

using System.ComponentModel.DataAnnotations;

public class RefreshTokenRequest
{

    [Required]
    public string RefreshToken { get; set; }
}