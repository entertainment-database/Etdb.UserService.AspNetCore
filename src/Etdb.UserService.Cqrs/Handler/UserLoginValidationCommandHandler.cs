using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Etdb.ServiceBase.Cqrs.Abstractions.Handler;
using Etdb.ServiceBase.Cryptography.Abstractions.Hashing;
using Etdb.UserService.Cqrs.Abstractions.Commands;
using Etdb.UserService.Domain.Entities;
using Etdb.UserService.Domain.Enums;
using Etdb.UserService.Presentation;
using Etdb.UserService.Repositories.Abstractions;
using Etdb.UserService.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Etdb.UserService.Cqrs.Handler
{
    public class
        UserLoginValidationCommandHandler : IResponseCommandHandler<UserLoginValidationCommand, UserLoginValidationDto>
    {
        private readonly IUsersService usersService;
        private readonly ILogger<UserLoginValidationCommandHandler> logger;
        private readonly IHasher hasher;
        private readonly ILoginLogRepository loginLogRepository;
        private static readonly UserLoginValidationDto FailedLogin = new UserLoginValidationDto(false);

        public UserLoginValidationCommandHandler(IUsersService usersService, IHasher hasher,
            ILoginLogRepository loginLogRepository, ILogger<UserLoginValidationCommandHandler> logger)
        {
            this.usersService = usersService;
            this.hasher = hasher;
            this.loginLogRepository = loginLogRepository;
            this.logger = logger;
        }

        public async Task<UserLoginValidationDto> Handle(UserLoginValidationCommand command,
            CancellationToken cancellationToken)
        {
            var user = await this.usersService.FindByUserNameOrEmailAsync(command.UserName);

            if (user == null)
            {
                return UserLoginValidationCommandHandler.FailedLogin;
            }

            var passwordIsValid = this.hasher.CreateSaltedHash(command.Password, user.Salt)
                                  == user.Password;

            if (passwordIsValid)
            {
                return new UserLoginValidationDto(true, user.Id);
            }

            await this.LogLoginEvent(LoginType.Failed, user.Id, command.IpAddress, "Given password is invalid!");
            return UserLoginValidationCommandHandler.FailedLogin;
        }

        private async Task LogLoginEvent(LoginType loginType, Guid userId, IPAddress ipAddress,
            string additionalInfo = null)
        {
            var log = new LoginLog(Guid.NewGuid(), userId, DateTime.UtcNow, loginType,
                ipAddress.ToString(), additionalInfo);

            try
            {
                await this.loginLogRepository.AddAsync(log);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, ex.Message);
            }
        }
    }
}