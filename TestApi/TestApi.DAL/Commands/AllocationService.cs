﻿using System;
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

        public AllocationService(ICommandHandler commandHandler, IQueryHandler queryHandler, ILogger<AllocationService> logger, IConfiguration config)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _logger = logger;
            _config = config;
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

                var userId = await CreateNewUser(userType, application, number);
                _logger.LogDebug($"A new user with Id {userId} has been created");

                await CreateNewAllocation(userId);
                _logger.LogDebug($"The new user with Id {userId} has a new allocation");

                user = await GetUserById(userId);
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

                if (allocation.ExpiresAt == null)
                {
                    return user;
                }

                if (!allocation.Allocated || allocation.ExpiresAt < DateTime.UtcNow)
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

        private async Task<Guid> CreateNewUser(UserType userType, Application application, int newNumber)
        {
            var emailStem = GetEmailStem();
            var request = new UserBuilder(emailStem, newNumber)
                .WithUserType(userType)
                .ForApplication(application)
                .BuildRequest();

            var createNewUserCommand = new CreateNewUserCommand
            (
                request.Username, request.ContactEmail, request.FirstName, request.LastName,
                request.DisplayName, request.Number, request.UserType, request.Application
            );

            await _commandHandler.Handle(createNewUserCommand);

            return createNewUserCommand.NewUserId;
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
