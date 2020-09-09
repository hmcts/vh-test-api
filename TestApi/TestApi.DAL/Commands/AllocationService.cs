using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TestApi.Common.Builders;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Helpers;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Contracts;

namespace TestApi.DAL.Commands
{
    public interface IAllocationService
    {
        /// <summary>
        ///     Allocate user service. This will re-use existing allocations entries before attempting to
        ///     create a new one.
        /// </summary>
        /// <param name="userType">Type of user to allocate (e.g. Judge)</param>
        /// <param name="application">Application to assign to (e.g. VideoWeb)</param>
        /// <param name="testType">Type of test user is required for. Default is Automation test</param>
        /// <param name="isProdUser">Whether the user will be required for prod environments</param>
        /// <param name="expiresInMinutes">Gives an expiry time in minutes. Default is 10 minutes</param>
        /// <returns>An allocated user</returns>
        Task<User> AllocateToService(UserType userType, Application application, TestType testType, bool isProdUser, int expiresInMinutes = 10);
    }

    public class AllocationService : IAllocationService
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IConfiguration _config;
        private readonly ILogger<AllocationService> _logger;
        private readonly IQueryHandler _queryHandler;
        private readonly IUserApiService _userApiService;

        public AllocationService(ICommandHandler commandHandler, IQueryHandler queryHandler,
            ILogger<AllocationService> logger,
            IConfiguration config, IUserApiService userApiService)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _logger = logger;
            _config = config;
            _userApiService = userApiService;
        }

        public async Task<User> AllocateToService(UserType userType, Application application, TestType testType, bool isProdUser, int expiresInMinutes = 10)
        {
            var users = await GetAllUsers(userType, application, isProdUser);
            _logger.LogDebug($"Found {users.Count} user(s) of type '{userType}' and application '{application}'");

            var allocations = await CreateAllocationsForUsersIfRequired(users);

            var userId = GetUnallocatedUserId(allocations);

            if (userId == null)
            {
                _logger.LogDebug($"All {users.Count} users were already allocated");

                var number = await IterateUserNumber(userType, application, isProdUser);
                _logger.LogDebug($"Iterated user number to {number}");

                await CreateNewUserInTestApi(userType, application, testType, isProdUser, number);
                _logger.LogDebug(
                    $"A new user with user type {userType}, application {application} and number {number} has been created");

                var newUser = await GetUserIdByUserTypeApplicationAndNumber(userType, application, number, isProdUser);
                _logger.LogDebug($"A new user with Id {newUser.Id} has been retrieved");

                await CreateNewAllocation(newUser.Id);
                _logger.LogDebug($"The new user with Id {newUser.Id} has a new allocation");

                userId = newUser.Id;
            }

            var user = await GetUserById(userId.Value);

            if (await UserDoesNotExistInAAD(user.ContactEmail))
            {
                _logger.LogDebug($"The user with username {user.Username} does not already exist in AAD");

                var response = await CreateUserInAAD(user);
                _logger.LogDebug($"The user with username {response.Username} created in AAD");
            }

            var adUserProfile = await GetADUserId(user.ContactEmail);
            _logger.LogDebug($"The ad user with Id {adUserProfile.User_id} has been retrieved");

            var groupsCount = await AddGroupsToUserIfRequired(user, adUserProfile);
            _logger.LogDebug($"The ad user has {groupsCount} groups");

            await AllocateUser(user.Id, expiresInMinutes);
            _logger.LogDebug($"User with username '{user.Username}' has been allocated");

            return user;
        }

        private async Task<List<User>> GetAllUsers(UserType userType, Application application, bool isProdUser)
        {
            var query = new GetAllUsersByUserTypeQuery(userType, application, isProdUser);
            return await _queryHandler.Handle<GetAllUsersByUserTypeQuery, List<User>>(query);
        }

        private async Task<List<Allocation>> CreateAllocationsForUsersIfRequired(IReadOnlyCollection<User> users)
        {
            var allocations = new List<Allocation>();

            foreach (var user in users)
            {
                var allocation = await GetAllocationByUserId(user.Id);

                if (allocation != null)
                {
                    allocations.Add(allocation);
                }
                else
                {
                    await CreateNewAllocation(user.Id);
                    allocations.Add(await GetAllocationByUserId(user.Id));
                    _logger.LogDebug($"The user with Id {user.Id} has a new allocation");
                }
            }

            _logger.LogDebug($"All {users.Count} users now have allocations");

            return allocations;
        }

        private async Task<Allocation> GetAllocationByUserId(Guid userId)
        {
            var query = new GetAllocationByUserIdQuery(userId);
            return await _queryHandler.Handle<GetAllocationByUserIdQuery, Allocation>(query);
        }

        private async Task CreateNewAllocation(Guid userId)
        {
            var command = new CreateNewAllocationByUserIdCommand(userId);
            await _commandHandler.Handle(command);
        }

        private static Guid? GetUnallocatedUserId(IEnumerable<Allocation> allocations)
        {
            foreach (var allocation in allocations)
                if (!allocation.IsAllocated())
                    return allocation.UserId;

            return null;
        }

        private async Task<int> IterateUserNumber(UserType userType, Application application, bool isProdUser)
        {
            var query = new GetNextUserNumberByUserTypeQuery(userType, application, isProdUser);
            return await _queryHandler.Handle<GetNextUserNumberByUserTypeQuery, Integer>(query);
        }

        private async Task<User> GetUserById(Guid userId)
        {
            var getUserByIdQuery = new GetUserByIdQuery(userId);
            return await _queryHandler.Handle<GetUserByIdQuery, User>(getUserByIdQuery);
        }

        private async Task CreateNewUserInTestApi(UserType userType, Application application, TestType testType, bool isProdUser, int newNumber)
        {
            var emailStem = GetEmailStem();

            var request = new UserBuilder(emailStem, newNumber)
                .WithUserType(userType)
                .ForApplication(application)
                .ForTestType(testType)
                .IsProdUser(isProdUser)
                .BuildRequest();

            var createNewUserCommand = new CreateNewUserCommand
            (
                request.Username, request.ContactEmail, request.FirstName, request.LastName,
                request.DisplayName, request.Number, request.TestType, request.UserType,
                request.Application, request.IsProdUser
            );

            await _commandHandler.Handle(createNewUserCommand);
        }

        private async Task<User> GetUserIdByUserTypeApplicationAndNumber(UserType userType, Application application,
            int number, bool isProdUser)
        {
            var query = new GetUserByUserTypeAppAndNumberQuery(userType, application, number, isProdUser);
            return await _queryHandler.Handle<GetUserByUserTypeAppAndNumberQuery, User>(query);
        }

        private async Task<bool> UserDoesNotExistInAAD(string contactEmail)
        {
            return !await _userApiService.CheckUserExistsInAAD(contactEmail);
        }

        private async Task<NewUserResponse> CreateUserInAAD(User user)
        {
            return await _userApiService.CreateNewUserInAAD(user.FirstName, user.LastName, user.ContactEmail, user.IsProdUser);
        }

        private async Task<UserProfile> GetADUserId(string contactEmail)
        {
            return await _userApiService.GetADUserProfile(contactEmail);
        }

        private async Task<int> AddGroupsToUserIfRequired(User user, UserProfile adUserProfile)
        {
            return await _userApiService.AddGroupsToUserIfRequired(user, adUserProfile);
        }

        private string GetEmailStem()
        {
            var emailStem = _config.GetValue<string>("UsernameStem");

            if (emailStem == null) throw new ConfigurationErrorsException("Email stem could not be retrieved");

            return emailStem;
        }

        private async Task AllocateUser(Guid userId, int expiresInMinutes)
        {
            var allocateCommand = new AllocateByUserIdCommand(userId, expiresInMinutes);
            await _commandHandler.Handle(allocateCommand);
        }
    }
}