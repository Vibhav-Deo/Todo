using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Todo.Backend.User.Services;
using Todo.Backend.User.Services.RequestResponse;
using Todo.Contracts.Api;
using Todo.Contracts.Commands.User;
using Todo.Contracts.StringResources;
using Todo.Database.Models;
using Todo.Web.RequestResponse.Token;
using Todo.Web.RequestResponse.User;
using Todo.Web.TokenManager;

namespace Todo.Web.Controllers
{
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserReadService _userReadService;
        private readonly ITokenManager _tokenManager;
        private readonly IMapper _mapper;
        private readonly IRequestClient<CreateUserCommand> _createUserCommandRequestClient;
        public UserController(ILogger<UserController> logger, IUserReadService userReadService, ITokenManager tokenManager, IBus bus, IRequestClient<CreateUserCommand> createUserCommand, IMapper mapper)
        {
            _logger = logger;
            _userReadService = userReadService;
            _tokenManager = tokenManager;
            _mapper = mapper;
            _createUserCommandRequestClient = createUserCommand;
        }

        /// <summary>
        /// In This method generates a JWT and add a user claim to Identity for future user. This method returns typeof(LoginResponse) on successfull response..
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ApiRoutes.User.LOGIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces(typeof(LoginResponse))]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return await MakeServiceCall(async () =>
            {
                var checUserCredentialsRequest = new CheckUserCredentialsRequest
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var checkUserCredentialsResponse = await _userReadService.IsValidUserCredentialsAsync(checUserCredentialsRequest);

                if (!checkUserCredentialsResponse.isValid)
                {
                    return Unauthorized();
                }

                var getUserRoleResponse = await _userReadService.GetUserRoleAsync(new GetUserRoleByEmailRequest
                {
                    Email = request.Email
                });

                var claims = new[]
                {
                    new Claim(ClaimTypes.Role, getUserRoleResponse.Role.ToString()),
                    new Claim(ClaimTypes.Email, request.Email),
                };

                var jwtResult = await _tokenManager.GenerateTokensAsync(request.Email, claims, DateTime.Now);

                _logger.LogInformation($"User [{request.Email}] logged in the system.");

                return Success(new LoginResponse
                {
                    Email = request.Email,
                    Role = getUserRoleResponse.Role.ToString(),
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString,
                    TokenType = "Bearer"
                });
            }, StringResources.GeneralError);
        }

        /// <summary>
        /// This method is used for user registeration upon success this method returns typeof(UserResponse).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ApiRoutes.User.REGISTER)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(typeof(User))]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return await MakeServiceCall(async () =>
            {
                var createUserCommand = new CreateUserCommand();

                _mapper.Map(request, createUserCommand);

                var user = await _createUserCommandRequestClient.GetResponse<User>(createUserCommand);

                return Created(user);

            }, StringResources.GeneralError);
        }

        [HttpGet]
        [Route(ApiRoutes.User.BY_EMAIL)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(typeof(UserResponse))]
        [Authorize]
        public async Task<IActionResult> GetUserByEmail([FromRoute] string userEmail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return await MakeServiceCall(async () =>
            {
                var user = await _userReadService.GetUserByEmail(new GetUserByEmailRequest
                {
                    Email = userEmail
                });

                var userResponse = _mapper.Map<UserResponse>(user.User);
                return Success(userResponse);

            }, StringResources.GeneralError);
        }

        /// <summary>
        /// This method is used to refresh the access token to keep the access token valid.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ApiRoutes.User.REFRESH_TOKEN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces(typeof(LoginResponse))]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            return await MakeServiceCall(async () =>
            {
                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var (principal, securityToken) = await _tokenManager.DecodeJwtTokenAsync(accessToken);

                var email = principal.FindFirst(claim => claim.Type == ClaimTypes.Email).Value;


                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                var jwtResult = await _tokenManager.RefreshAsync(request.RefreshToken, accessToken, DateTime.Now);

                _logger.LogInformation($"User [{email}] has refreshed JWT token.");


                return Success(new LoginResponse
                {
                    Email = email,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString,
                    TokenType = "Bearer"
                });
            }, StringResources.GeneralError);
        }

        /// <summary>
        /// This method removes the refresh token and invalidates the login of the current logged in user. 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(ApiRoutes.User.LOG_OUT)]
        [Produces(typeof(string))]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // optionally "revoke" JWT token on the server side --> add the current token to a block-list
            // https://github.com/auth0/node-jsonwebtoken/issues/375

            return await MakeServiceCall(async () =>
            {
                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");

                await _tokenManager.RemoveRefreshTokenByAccessTokenAsync(accessToken);

                return Success(StringResources.LogoutSuccess);
            }, StringResources.GeneralError);

        }
    }
}
