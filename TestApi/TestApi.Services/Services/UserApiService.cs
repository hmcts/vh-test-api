﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using TestApi.Common.Configuration;
using TestApi.Common.Data;
using TestApi.Contract.Dtos;
using TestApi.Contract.Enums;
using TestApi.Services.Helpers;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace TestApi.Services.Services
{
    public interface IUserApiService
    {
        /// <summary>Checks if a user already exists based on their username</summary>
        /// <param name="username">username of the user</param>
        /// <returns>True if the user exists in AAD</returns>
        Task<bool> CheckUserExistsInAAD(string username);

        /// <summary>Creates a user based on the user information</summary>
        /// <param name="firstName">First name of the user</param>
        /// <param name="lastName">Last name of the user</param>
        /// <param name="contactEmail">Contact email of the user</param>
        /// <param name="isProdUser">Is the user required for prod</param>
        /// <returns>New user details</returns>
        Task<NewUserResponse> CreateNewUserInAAD(string firstName, string lastName, string contactEmail, bool isProdUser);

        /// <summary>Deletes a user by username</summary>
        /// <param name="username">Username of the user</param>
        /// <returns></returns>
        Task DeleteUserInAAD(string username);

        /// <summary>Adds required groups to the test user</summary>
        /// <param name="user">The test api user profile</param>
        /// <param name="adUserId">The AD user profile id</param>
        /// <returns>A count of the number of groups the user now has</returns>
        Task<int> AddGroupsToUser(UserDto user, string adUserId);
    }

    public class UserApiService : IUserApiService
    {
        protected const int POLLY_RETRIES = 4;
        private readonly IUserApiClient _userApiClient;
        private readonly UserGroupsConfiguration _userGroups;
        private readonly ILogger<UserApiService> _logger;

        public UserApiService(IUserApiClient userApiClient, IOptions<UserGroupsConfiguration> userGroupsConfiguration, ILogger<UserApiService> logger)
        {
            _userApiClient = userApiClient;
            _userGroups = userGroupsConfiguration.Value;
            _logger = logger;
            ValidateGroupsAreSet(userGroupsConfiguration.Value);
        }

        private static void ValidateGroupsAreSet(UserGroupsConfiguration values)
        {
            values.GetType().GetProperties()
                .Where(pi => pi.PropertyType == typeof(string))
                .Select(pi => (string)pi.GetValue(values))
                .Any(string.IsNullOrWhiteSpace)
                .Should().BeFalse("All user group values are set");
        }

        public async Task<bool> CheckUserExistsInAAD(string username)
        {
            var policy = Policy
                .Handle<UserApiException>(ex => ex.StatusCode.Equals(HttpStatusCode.InternalServerError))
                .WaitAndRetryAsync(POLLY_RETRIES, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            try
            {
                await policy.ExecuteAsync(async () => await _userApiClient.GetUserByAdUserNameAsync(username));
            }
            catch (UserApiException e)
            {
                if (e.StatusCode == (int) HttpStatusCode.NotFound) return false;
                _logger.LogError(e, "{exceptionCode} exception occured with message '{message}' whilst trying to check if user exist in AAD with username '{username}'", e.StatusCode, e.Message, username);
                throw;
            }

            return true;
        }

        public async Task<NewUserResponse> CreateNewUserInAAD(string firstName, string lastName, string contactEmail, bool isProdUser)
        {
            const string BLANK = " ";
            const string UNDERSCORE = "_";

            var createUserRequest = new CreateUserRequest
            {
                FirstName = firstName.Replace(BLANK, string.Empty),
                LastName = lastName.Replace(BLANK, UNDERSCORE),
                RecoveryEmail = contactEmail,
                IsTestUser = true
            };

           return await _userApiClient.CreateUserAsync(createUserRequest);
        }

        public async Task DeleteUserInAAD(string username)
        {
            var policy = Policy
                .Handle<UserApiException>(ex => ex.StatusCode.Equals(HttpStatusCode.NotFound))
                .WaitAndRetryAsync(POLLY_RETRIES, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            try
            {
                await policy.ExecuteAsync(async () => await _userApiClient.DeleteUserAsync(username));
            }
            catch (UserApiException e)
            {
                if (e.StatusCode == (int) HttpStatusCode.InternalServerError)
                {
                    _logger.LogError(e, "{exceptionCode} exception occured with message '{message}' whilst trying to delete a user in AAD with username '{username}'", e.StatusCode, e.Message, username);
                    throw;
                }
            }
        }

        public async Task<int> AddGroupsToUser(UserDto user, string adUserId)
        {
            var requiredGroups = GetRequiredGroups(user);

            foreach (var requiredGroup in requiredGroups)
            {
                await AddUserToGroup(adUserId, requiredGroup);
            }

            _logger.LogInformation("{username} added to {count} groups", user.Username, requiredGroups.Count);

            return requiredGroups.Count;
        }

        private List<string> GetRequiredGroups(UserDto user)
        {
            var userGroupStrategies = new UserGroups().GetStrategies(_userGroups);
            var groups = userGroupStrategies[user.UserType].GetGroups();

            if (user.IsProdUser && (user.UserType == UserType.Judge || user.UserType == UserType.VideoHearingsOfficer))
            {
                groups.AddRange(ConvertGroupsStringToList.Convert(_userGroups.KinlyProdGroups));
            }

            if (!user.IsProdUser || user.UserType != UserType.Judge)
            {
                groups.AddRange(ConvertGroupsStringToList.Convert(_userGroups.TestAccountGroups));
            }

            if (IsPerformanceTestUser(user.FirstName)) groups.AddRange(ConvertGroupsStringToList.Convert(_userGroups.PerformanceTestAccountGroups));

            _logger.LogInformation("{count} groups are required for {username}", groups.Count, user.Username);

            return groups;
        }

        private static bool IsPerformanceTestUser(string firstName)
        {
            return firstName.Contains(UserData.PERFORMANCE_FIRST_NAME_PREFIX);
        }

        private async Task AddUserToGroup(string adUserId, string group)
        {
            var request = new AddUserToGroupRequest
            {
                UserId = adUserId,
                GroupName = group
            };

            await PollToAddUserToGroup(request);
        }

        private async Task PollToAddUserToGroup(AddUserToGroupRequest request)
        {
            var policy = Policy
                .Handle<UserApiException>(ex => ex.StatusCode.Equals(HttpStatusCode.NotFound))
                .WaitAndRetryAsync(POLLY_RETRIES, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            
            try
            {
                await policy.ExecuteAsync(async () => await _userApiClient.AddUserToGroupAsync(request));
            }
            catch (UserApiException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.InternalServerError)
                {
                    _logger.LogError(e, "{exceptionCode} exception occured with message '{message}' whilst trying to add a group to a user in AAD with username '{userId}'", e.StatusCode, e.Message, request.UserId);
                    throw;
                }
            }
        }
    }
}