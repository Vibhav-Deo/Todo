using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Todo.Backend.User.Repository;
using Todo.Backend.User.Services.RequestResponse;
using Todo.Contracts.Exceptions;
using Todo.Contracts.StringResources;

namespace Todo.Backend.User.Services
{
    public interface IUserReadService
    {
        Task<CheckUserCredentialsResponse> IsValidUserCredentialsAsync(CheckUserCredentialsRequest request);
        Task<GetUserRoleByEmailResponse> GetUserRoleAsync(GetUserRoleByEmailRequest request);
        Task<GetUserByIdResponse> GetUserAsync(GetUserByIdRequest request);
        Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest request);
    }

    public class UserReadService : IUserReadService
    {
        private readonly ILogger<UserReadService> _logger;
        private readonly IUserReadRepository _userReadRepository;
        public UserReadService(IUserReadRepository userReadRepository, ILogger<UserReadService> logger)
        {
            _logger = logger;
            _userReadRepository = userReadRepository;
        }

        public async Task<CheckUserCredentialsResponse> IsValidUserCredentialsAsync(CheckUserCredentialsRequest request)
        {
            _logger.LogInformation($"Validating user [{request.Email}]");

            bool v = await IsAnExistingUserAsync(request.Email);
            if (!v)
            {
                return new CheckUserCredentialsResponse
                {
                    isValid = false
                };
            }

            var user = await _userReadRepository.GetUserByEmailAsync(request.Email);

            byte[] hash = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(request.Password));
            StringBuilder sb = new StringBuilder();

            foreach (var passwordByte in hash)
            {
                sb.Append(passwordByte.ToString("x2"));
            }

            return new CheckUserCredentialsResponse
            {
                isValid = sb.Equals(user.PasswordHash) && user.Email.Equals(request.Email)
            };
        }

        private async Task<bool> IsAnExistingUserAsync(string email)
        {
            return await _userReadRepository.IsExistingUserAsync(email);
        }

        private async Task<bool> IsAnExistingUserAsync(Guid userId)
        {
            return await _userReadRepository.IsExistingUserAsync(userId);
        }

        public async Task<GetUserRoleByEmailResponse> GetUserRoleAsync(GetUserRoleByEmailRequest request)
        {
            if (!await IsAnExistingUserAsync(request.Email))
            {
                throw new TodoApplicationException(StringResources.UserNotFound, StatusCodes.Status404NotFound, new Exception(StringResources.UserNotFound));
            }
            return new GetUserRoleByEmailResponse
            {
                Role = await _userReadRepository.GetUserRoleByEmailAsync(request.Email)
            };

        }

        /*        public async Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request)
                {
                    if (await IsAnExistingUserAsync(request.Email))
                    {
                        throw new LandmarkRemarkApplicationException(StringResources.UserAlreadyRegistered, StatusCodes.Status409Conflict);
                    }

                    if (string.IsNullOrEmpty(request.Password) || string.IsNullOrWhiteSpace(request.Password))
                    {
                        throw new ValidationException(StringResources.PasswordCannotBeEmty, StatusCodes.Status400BadRequest);
                    }

                    Guid userId = Guid.NewGuid();
                    byte[] hash = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(request.Password));
                    StringBuilder sb = new StringBuilder();
                    foreach (var passwordByte in hash)
                    {
                        sb.Append(passwordByte.ToString("x2"));
                    }

                    try
                    {
                        await _userRepository.RegisterUserAsync(new User
                        {
                            Id = userId,
                            City = request.City,
                            Country = request.Country,
                            Email = request.Email,
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            PhoneNumber = request.PhoneNumber,
                            Role = (int)UserRoles.NormalUser,
                            State = request.State,
                            UserName = request.UserName,
                            Postcode = request.Postcode,
                            Street = request.Street,
                            PasswordHash = sb.ToString()
                        });
                    }
                    catch (Exception)
                    {

                        throw new LandmarkRemarkApplicationException(StringResources.FailedToAddUser, StatusCodes.Status500InternalServerError);
                    }

                    return new RegisterUserResponse
                    {
                        CreatedUser = await _userRepository.GetUserAsync(userId)
                    };
                }*/

        public async Task<GetUserByIdResponse> GetUserAsync(GetUserByIdRequest request)
        {
            if (!await IsAnExistingUserAsync(request.UserId))
            {
                throw new TodoApplicationException(StringResources.UserNotFound, StatusCodes.Status404NotFound, new Exception(StringResources.UserNotFound));
            }

            return new GetUserByIdResponse
            {
                User = await _userReadRepository.GetUserAsync(request.UserId)
            };
        }

        public async Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest request)
        {
            var response = await _userReadRepository.GetUserByEmailAsync(request.Email);
            return new GetUserByEmailResponse
            {
                User = response
            };
        }
    }
}
