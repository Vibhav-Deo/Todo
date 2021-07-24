using AutoMapper;
using MassTransit;
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
using Todo.Web.RequestResponse.User;
using Todo.Web.TokenManager;

namespace Todo.Web.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserReadService _userReadService;
        private readonly ITokenManager _tokenManager;
        private readonly IMapper _mapper;
        private readonly IBus _bus;
        public UserController(ILogger<UserController> logger, IUserReadService userReadService, ITokenManager tokenManager, IBus bus,IMapper mapper)
        {
            _logger = logger;
            _userReadService = userReadService;
            _tokenManager = tokenManager;
            _mapper = mapper;
            _bus = bus;
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
        [Produces(typeof(string))]
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

                await _bus.Publish(createUserCommand);

                return Success(StringResources.UserRegisterationSuccessful);

            }, StringResources.GeneralError);
        }
    }
}
