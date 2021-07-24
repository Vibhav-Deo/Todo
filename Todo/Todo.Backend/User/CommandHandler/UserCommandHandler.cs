using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Todo.Backend.User.Repositories.Write;
using Todo.Contracts.Commands.User;
using Todo.Contracts.Enums;
using Todo.Contracts.Events.User;
using Todo.Contracts.Exceptions;
using Todo.Contracts.StringResources;
using Models = Todo.Database.Models;

namespace Todo.Backend.User.CommandHandler
{
    public class UserCommandHandler : IConsumer<CreateUserCommand>, IConsumer<UpdateUserCommand>, IConsumer<DeleteUserCommand>
    {
        private const string entity = "User";
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly ILogger<UserCommandHandler> _logger;
        private readonly IBus _bus;
        private readonly IPublishEndpoint _publishEndpoint;
        public UserCommandHandler(IUserWriteRepository userWriteRepository, IBus bus, IPublishEndpoint publishEndpoint, ILogger<UserCommandHandler> logger)
        {
            _userWriteRepository = userWriteRepository;
            _bus = bus;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<CreateUserCommand> context)
        {
            var command = context.Message;
            try
            {
                if (await _userWriteRepository.IsExistingUserAsync(command.Email))
                {
                    throw new Exception(StringResources.UserAlreadyRegistered);
                }

                if (string.IsNullOrEmpty(command.Password) || string.IsNullOrWhiteSpace(command.Password))
                {
                    throw new Exception(StringResources.PasswordCannotBeEmpty);
                }

                Guid userId = context.MessageId ?? Guid.NewGuid();
                byte[] hash = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(command.Password));
                StringBuilder sb = new StringBuilder();
                foreach (var passwordByte in hash)
                {
                    sb.Append(passwordByte.ToString("x2"));
                }

                using (var transactionScope = new TransactionScope())
                {
/*                    await _userWriteRepository.RegisterUserAsync(new Models.User
                    {
                        Id = userId,
                        City = command.City,
                        Country = command.Country,
                        Email = command.Email,
                        FirstName = command.FirstName,
                        LastName = command.LastName,
                        PhoneNumber = command.PhoneNumber,
                        Role = UserRoles.NormalUser,
                        State = command.State,
                        UserName = command.UserName,
                        Postcode = command.Postcode,
                        Street = command.Street,
                        PasswordHash = sb.ToString()
                    });*/
                    transactionScope.Complete();
                }


                var correlationId = context.CorrelationId ?? Guid.Empty;
                string entityId = userId.ToString();
                
                var message = new UserCreatedEvent(entityId, entity, EntityType.User, correlationId ,DateTimeOffset.UtcNow);
                await _bus.Publish(message);

            }
            catch (Exception exception)
            {
                throw new TodoApplicationException(StringResources.FailedToAddUser, StatusCodes.Status500InternalServerError, exception.InnerException);
            }
        }

        public Task Consume(ConsumeContext<UpdateUserCommand> context)
        {
            throw new NotImplementedException();
        }

        public Task Consume(ConsumeContext<DeleteUserCommand> context)
        {
            throw new NotImplementedException();
        }
    }
}
