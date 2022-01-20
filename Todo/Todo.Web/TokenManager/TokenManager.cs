namespace Todo.Web.TokenManager;

using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Todo.Web.Infrastructure;
using Todo.Web.RequestResponse.Token;

public interface ITokenManager
{
    Task<TokenAuthResponse> GenerateTokensAsync(string email, Claim[] claims, DateTime now);
    Task<TokenAuthResponse> RefreshAsync(string refreshToken, string accessToken, DateTime now);
    Task RemoveExpiredRefreshTokensAsync(DateTime now);
    Task RemoveRefreshTokenByAccessTokenAsync(string accessToken);
    Task<(ClaimsPrincipal, JwtSecurityToken)> DecodeJwtTokenAsync(string token);
}

public class TokenManager : ITokenManager
{
    private readonly ConcurrentDictionary<string, RefreshTokenResponse> _usersRefreshTokens;  // can store in a database or a distributed cache
    private readonly TokenConfig _jwtTokenConfig;
    private readonly byte[] _secret;
    private readonly ILogger<TokenManager> _logger;

    public TokenManager(TokenConfig jwtTokenConfig, ILogger<TokenManager> logger)
    {
        _jwtTokenConfig = jwtTokenConfig;
        _usersRefreshTokens = new ConcurrentDictionary<string, RefreshTokenResponse>();
        _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        _logger = logger;
    }

    /// <summary>
    /// This method takes an email, user claims and date time and generates a JWT for user authentication. It returns access token and refresh token
    /// </summary>
    /// <param name="email"></param>
    /// <param name="claims"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    public async Task<TokenAuthResponse> GenerateTokensAsync(string email, Claim[] claims, DateTime now)
    {
        _logger.LogInformation($"Token generation start");

        var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);

        var jwtToken = new JwtSecurityToken(
            _jwtTokenConfig.Issuer,
            shouldAddAudienceClaim ? _jwtTokenConfig.Audience : string.Empty,
            claims,
            expires: now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        var refreshToken = new RefreshTokenResponse
        {
            Email = email,
            TokenString = await GenerateRefreshTokenStringAsync(),
            ExpireAt = now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration)
        };

        _usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (s, t) => refreshToken);

        _logger.LogInformation($"Token generation finished");

        return new TokenAuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    /// <summary>
    /// Method to generate refresh token.
    /// </summary>
    /// <returns></returns>
    private static Task<string> GenerateRefreshTokenStringAsync()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Task.FromResult(Convert.ToBase64String(randomNumber));
    }

    /// <summary>
    /// This method cleans a up expired access tokens.
    /// </summary>
    /// <param name="now"></param>
    /// <returns></returns>
    public Task RemoveExpiredRefreshTokensAsync(DateTime now)
    {
        var expiredTokens = _usersRefreshTokens.Where(x => x.Value.ExpireAt < now).ToList();
        foreach (var expiredToken in expiredTokens)
        {
            _usersRefreshTokens.TryRemove(expiredToken.Key, out _);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// THis method removes the refresh token to terminate current user session, TODO: Find out how to invalidate JWT, it does not seem easy otu of scope for this?
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public async Task RemoveRefreshTokenByAccessTokenAsync(string accessToken)
    {
        var (principal, jwtToken) = await DecodeJwtTokenAsync(accessToken);

        var email = principal.FindFirst(claim => claim.Type == ClaimTypes.Email).Value;

        var refreshTokens = _usersRefreshTokens.Where(x => x.Value.Email == email).ToList();

        foreach (var refreshToken in refreshTokens)
        {
            _usersRefreshTokens.TryRemove(refreshToken.Key, out _);
        }
        return;
    }

    /// <summary>
    /// This method refreshes the access token so that it has more life time.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="accessToken"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    public async Task<TokenAuthResponse> RefreshAsync(string refreshToken, string accessToken, DateTime now)
    {
        var (principal, jwtToken) = await DecodeJwtTokenAsync(accessToken);

        _logger.LogInformation($"Refresh token start");

        if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
        {
            throw new SecurityTokenException("Invalid token");
        }

        var email = principal.FindFirst(claim => claim.Type == ClaimTypes.Email).Value;

        _logger.LogInformation($"User [{email}] is trying to refresh JWT token.");

        if (!_usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
        {
            throw new SecurityTokenException("Invalid token");
        }
        if (existingRefreshToken.Email != email || existingRefreshToken.ExpireAt < now)
        {
            throw new SecurityTokenException("Invalid token");
        }

        return await GenerateTokensAsync(email, principal.Claims.ToArray(), now); // need to recover the original claims
    }

    /// <summary>
    /// This method is used to decode access token to get the user parameters like email and role.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<(ClaimsPrincipal, JwtSecurityToken)> DecodeJwtTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new SecurityTokenException("Invalid token");
        }
        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_secret),
                    ValidAudience = _jwtTokenConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                },
                out var validatedToken);
        return Task.FromResult((principal, validatedToken as JwtSecurityToken));
    }
}