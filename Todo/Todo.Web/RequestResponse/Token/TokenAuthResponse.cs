namespace Todo.Web.RequestResponse.Token;

using System.Text.Json.Serialization;

public class TokenAuthResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refreshToken")]
    public RefreshTokenResponse RefreshToken { get; set; }
}