namespace Todo.Web.RequestResponse.Token;

using System;

public class RefreshTokenResponse
{
    public string Email { get; set; }
    public string TokenString { get; set; }
    public DateTime ExpireAt { get; set; }
}