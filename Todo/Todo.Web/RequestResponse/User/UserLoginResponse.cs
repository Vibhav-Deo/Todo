namespace Todo.Web.RequestResponse.User
{
    public class LoginResponse
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
    }
}
