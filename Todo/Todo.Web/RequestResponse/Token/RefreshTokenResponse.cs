using System;

namespace Todo.Web.RequestResponse.Token
{
    public class RefreshTokenResponse
    {
        public string Email { get; set; }
        public string TokenString { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
