using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TestApi.Common.Builders;
using TestApi.Contract.Responses;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Helpers;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Contracts;
using CreateUserRequest = TestApi.Contract.Requests.CreateUserRequest;

namespace TestApi.DAL.Commands
{
    public interface IAllocationService
    {
        /// <summary>
        /// Allocate user service. This will re-use existing allocations entries before attempting to
        /// create a new one.
        /// </summary>
        /// <param name="userType">Type of user to allocate (e.g. Judge)</param>
        /// <param name="application">Application to assign to (e.g. VideoWeb)</param>
        /// <param name="expiresInMinutes">Gives an expiry time in minutes. Default is 10 minutes</param>
        /// <returns>An allocated user</returns>
        Task<User> AllocateToService(UserType userType, Application application, int expiresInMinutes = 10);
    }

    public class AllocationService : IAllocationService
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<AllocationService> _logger;
        private readonly IConfiguration _config;
        private readonly IUserApiService _userApiService;
        private CreateUserRequest _newUserRequest;

        public AllocationService(ICommandHandler commandHandler, IQueryHandler queryHandler, ILogger<AllocationService> logger, 
            IConfiguration config, IUserApiService userApiService)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _logger = logger;
            _config = config;
            _userApiService = userApiService;
        }

        public async Task<User> AllocateToService(UserType userType, Application application, int expiresInMinutes)
        {
            var users = await GetAllUsersByUserTypeAndApplication(userType, application);
            _logger.LogDebug($"Found {users.Count} user(s) of type '{userType}' and application '{application}'");

            await CreateAllocationsForUsersIfRequired(users);

            if (users.Count > 0)
            {
                _logger.LogDebug($"All {users.Count} users now have allocations");
            }

            var user = await GetUnallocatedUser(users);

            if (user == null)
            {
                _logger.LogDebug($"All {users.Count} users were already allocated");

                var number = await IterateUserNumber(userType, application);
                _logger.LogDebug($"Iterated user number to {number}");

                var userId = await CreateNewUserInTestApi(userType, application, number);
                _logger.LogDebug($"A new user with Id {userId} has been created");

                user = await GetUserById(userId);

                await CreateNewAllocation(userId);
                _logger.LogDebug($"The new user with Id {userId} has a new allocation");
            }

            if (await UserDoesNotExistInAAD(user.ContactEmail))
            {
                _logger.LogDebug($"The user with username {user.Username} does not already exist in AAD");

                var response = await CreateUserInAAD();
                _logger.LogDebug($"The user with username {response.Username} created in AAD");
            }

            await AllocateUser(user.Id, expiresInMinutes);
            _logger.LogDebug($"User with username '{user.Username}' has been allocated");

            return user;
        }

        private async Task<List<User>> GetAllUsersByUserTypeAndApplication(UserType userType, Application application)
        {
            var getAllUsersByUserTypeQuery = new GetAllUsersByUserTypeQuery(userType, application);
            return await _queryHandler.Handle<GetAllUsersByUserTypeQuery, List<User>>(getAllUsersByUserTypeQuery);
        }

        private async Task CreateAllocationsForUsersIfRequired(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                var allocation = await GetAllocationByUserId(user.Id);

                if (allocation != null) continue;
                await CreateNewAllocation(user.Id);
                _logger.LogDebug($"The user with Id {user.Id} has a new allocation");
            }
        }

        private async Task<Allocation> GetAllocationByUserId(Guid userId)
        {
            var getAllocationByUserIdQuery = new GetAllocationByUserIdQuery(userId);
            return await _queryHandler.Handle<GetAllocationByUserIdQuery, Allocation>(getAllocationByUserIdQuery);
        }

        private async Task CreateNewAllocation(Guid userId)
        {
            var createNewAllocationCommand = new CreateNewAllocationByUserIdCommand(userId); 
            await _commandHandler.Handle(createNewAllocationCommand);
        }

        private async Task<User> GetUnallocatedUser(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                var allocation = await GetAllocationByUserId(user.Id);

                if (allocation.IsAllocated() == false)
                {
                    return user;
                }
            }

            return null;
        }

        private async Task<int> IterateUserNumber(UserType userType, Application application)
        {
            var getHighestNumberQuery = new GetNextUserNumberByUserTypeQuery(userType, application);
            return await _queryHandler.Handle<GetNextUserNumberByUserTypeQuery, Integer>(getHighestNumberQuery);
        }

        private async Task<User> GetUserById(Guid userId)
        {
            var getUserByIdQuery = new GetUserByIdQuery(userId);
            return await _queryHandler.Handle<GetUserByIdQuery, User>(getUserByIdQuery);
        }

        private async Task<Guid> CreateNewUserInTestApi(UserType userType, Application application, int newNumber)
        {
            var emailStem = GetEmailStem();
            _newUserRequest = new UserBuilder(emailStem, newNumber)
                .WithUserType(userType)
                .ForApplication(application)
                .BuildRequest();

            var createNewUserCommand = new CreateNewUserCommand
            (
                _newUserRequest.Username, _newUserRequest.ContactEmail, _newUserRequest.FirstName, _newUserRequest.LastName,
                _newUserRequest.DisplayName, _newUserRequest.Number, _newUserRequest.UserType, _newUserRequest.Application
            );

            await _commandHandler.Handle(createNewUserCommand);

            return createNewUserCommand.NewUserId;
        }

        private async Task<bool> UserDoesNotExistInAAD(string contactEmail)
        {
            return !await _userApiService.CheckUserExistsInAAD(contactEmail);
        }

        private async Task<NewUserResponse> CreateUserInAAD()
        {
            var user = new ADUserBuilder(_newUserRequest).BuildUser();
            return await _userApiService.CreateNewUserInAAD(user);
        }

        private string GetEmailStem()
        {
            var emailStem = _config.GetValue<string>("UsernameStem");

            if (emailStem == null)
            {
                throw new ConfigurationErrorsException("Email stem could not be retrieved");
            }

            return emailStem;
        }

        private async Task AllocateUser(Guid userId, int expiresInMinutes)
        {
            var allocateCommand = new AllocateByUserIdCommand(userId, expiresInMinutes);
            await _commandHandler.Handle(allocateCommand);
        }
    }
}
