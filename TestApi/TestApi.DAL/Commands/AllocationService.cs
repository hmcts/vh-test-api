﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using TestApi.Common.Builders;
using TestApi.Contract.Dtos;
using TestApi.Contract.Enums;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Helpers;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Services.Services;
using UserApi.Contract.Responses;

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
        /// <param name="allocatedBy">Allocated By a particular user</param>
        /// <returns>An allocated user</returns>
        Task<UserDto> AllocateToService(UserType userType, Application application, TestType testType, bool isProdUser, int expiresInMinutes = 10, string allocatedBy = null);

        /// <summary>
        ///     Allocate eJudicial Judges, Panel Members and Wingers from the pool of available users
        /// </summary>
        /// <param name="userType">Type of user to allocate (e.g. Judge)</param>
        /// <param name="testType">Type of test user is required for. Default is Automation test</param>
        /// <param name="expiresInMinutes">Gives an expiry time in minutes. Default is 10 minutes</param>
        /// <param name="allocatedBy">Allocated By a particular user</param>
        /// <returns>An allocated user</returns>
        Task<UserDto> AllocateJudicialOfficerHolderToService(TestType testType, int expiresInMinutes = 10, string allocatedBy = null);
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

        public async Task<UserDto> AllocateToService(UserType userType, Application application, TestType testType, bool isProdUser, int expiresInMinutes = 10, string allocatedBy = null)
        {
            var users = await GetAllUsers(userType, testType, application, isProdUser);
            _logger.LogDebug($"Found {users.Count} user(s) of type '{userType}', test type '{testType}' and application '{application}'");

            var allocations = await CreateAllocationsForUsersIfRequired(users);

            var userId = GetUnallocatedUserId(allocations);

            if (userId == null)
            {
                _logger.LogDebug($"All {users.Count} users were already allocated");

                var number = await IterateUserNumber(userType, application, isProdUser, testType);
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

            if (!await IsRecentlyCreated(user.Username) && await UserDoesNotExistInAAD(user.Username))
            {
                _logger.LogDebug($"The user with username {user.Username} does not already exist in AAD");

                var response = await CreateUserInAAD(user);
                _logger.LogDebug($"The user with username {response.Username} created in AAD");

                if (response.Username != user.Username)
                {
                    await _userApiService.DeleteUserInAAD(response.Username);
                    _logger.LogDebug($"The newly created user was a duplicate of an existing AAD user and was therefore deleted");
                }
                else
                {
                    await AddNewUserToRecentlyCreatedList(user.Username);
                    _logger.LogDebug($"The ad user has been added to the list of recently created users with username {user.Username}");

                    var groupsCount = await AddGroupsToUser(user, response.UserId);
                    _logger.LogDebug($"The ad user now has {groupsCount} groups");
                }
            }

            await AllocateUser(user.Id, expiresInMinutes, allocatedBy);
            _logger.LogDebug($"User with username '{user.Username}' has been allocated");

            return user;
        }

        public async Task<UserDto> AllocateJudicialOfficerHolderToService(TestType testType, int expiresInMinutes = 10, string allocatedBy = null)
        {
            var users = await GetAllUsers(UserType.Judge, testType, Application.Ejud, false);
            _logger.LogDebug($"Found {users.Count} JOH user(s) with test type '{testType}'");

            if (users.Count.Equals(0))
            {
                throw new NoEjudUsersExistException();
            }

            var allocations = await GetAllocationsForUsers(users);

            var userId = GetUnallocatedUserId(allocations);

            if (userId == null)
            {
                throw new AllUsersAreAllocatedException();
            }

            var user = await GetUserById(userId.Value);

            await AllocateUser(user.Id, expiresInMinutes, allocatedBy);
            _logger.LogDebug($"User with username '{user.Username}' has been allocated");

            return user;
        }

        private async Task AddNewUserToRecentlyCreatedList(string username)
        {
            var command = new CreateNewRecentUserByUsernameCommand(username);
            await _commandHandler.Handle(command);
        }

        private async Task<bool> IsRecentlyCreated(string username)
        {
            var query = new GetRecentUserByUsernameQuery(username);
            var recentUser = await _queryHandler.Handle<GetRecentUserByUsernameQuery, RecentUser>(query);

            if (recentUser == null)
            {
                return false;
            }

            if (recentUser.IsRecentlyCreated())
            {
                return true;
            }

            var command = new DeleteNewRecentUserByUsernameCommand(username);
            await _commandHandler.Handle(command);

            return false;
        }

        private async Task<List<UserDto>> GetAllUsers(UserType userType, TestType testType, Application application, bool isProdUser)
        {
            var query = new GetAllUsersByFilterQuery(userType, testType, application, isProdUser);
            return await _queryHandler.Handle<GetAllUsersByFilterQuery, List<UserDto>>(query);
        }

        private async Task<List<Allocation>> CreateAllocationsForUsersIfRequired(IReadOnlyCollection<UserDto> users)
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

        private async Task<List<Allocation>> GetAllocationsForUsers(IEnumerable<UserDto> users)
        {
            var allocations = new List<Allocation>();

            foreach (var user in users)
            {
                var allocation = await GetAllocationByUserId(user.Id);

                if (allocation != null)
                {
                    allocations.Add(allocation);
                }
            }

            _logger.LogDebug($"{allocations.Count} allocations found");

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

        private async Task<int> IterateUserNumber(UserType userType, Application application, bool isProdUser, TestType testType)
        {
            var query = new GetNextUserNumberByUserTypeQuery(userType, application, isProdUser, testType);
            return await _queryHandler.Handle<GetNextUserNumberByUserTypeQuery, Integer>(query);
        }

        private async Task<UserDto> GetUserById(Guid userId)
        {
            var getUserByIdQuery = new GetUserByIdQuery(userId);
            return await _queryHandler.Handle<GetUserByIdQuery, UserDto>(getUserByIdQuery);
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

        private async Task<UserDto> GetUserIdByUserTypeApplicationAndNumber(UserType userType, Application application,
            int number, bool isProdUser)
        {
            var query = new GetUserByUserTypeAppAndNumberQuery(userType, application, number, isProdUser);
            return await _queryHandler.Handle<GetUserByUserTypeAppAndNumberQuery, UserDto>(query);
        }

        private async Task<bool> UserDoesNotExistInAAD(string username)
        {
            return !await _userApiService.CheckUserExistsInAAD(username);
        }

        private async Task<NewUserResponse> CreateUserInAAD(UserDto user)
        {
            return await _userApiService.CreateNewUserInAAD(user.FirstName, user.LastName, user.ContactEmail, user.IsProdUser);
        }

        private async Task<int> AddGroupsToUser(UserDto user, string adUserId)
        {
            return await _userApiService.AddGroupsToUser(user, adUserId);
        }

        private string GetEmailStem()
        {
            var emailStem = _config.GetValue<string>("UsernameStem");

            if (emailStem == null) throw new ConfigurationErrorsException("Email stem could not be retrieved");

            return emailStem;
        }

        private async Task AllocateUser(Guid userId, int expiresInMinutes, string allocatedBy = null)
        {
            var allocateCommand = new AllocateByUserIdCommand(userId, expiresInMinutes, allocatedBy);
            await _commandHandler.Handle(allocateCommand);
        }
    }
}