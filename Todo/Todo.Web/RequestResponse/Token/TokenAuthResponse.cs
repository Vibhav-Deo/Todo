using System.Text.Json.Serialization;

namespace Todo.Web.RequestResponse.Token
{
    public class TokenAuthResponse
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public RefreshTokenResponse RefreshToken { get; set; }
    }
}
