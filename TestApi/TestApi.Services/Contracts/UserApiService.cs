using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Polly;
using TestApi.Common.Configuration;
using TestApi.Common.Data;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Helpers;

namespace TestApi.Services.Contracts
{
    public interface IUserApiService
    {
        /// <summary>Checks if a user already exists based on their contact email</summary>
        /// <param name="contactEmail">Contact email of the user</param>
        /// <returns>True if the user exists in AAD</returns>
        Task<bool> CheckUserExistsInAAD(string contactEmail);

        /// <summary>Creates a user based on the user information</summary>
        /// <param name="firstName">First name of the user</param>
        /// <param name="lastName">Last name of the user</param>
        /// <param name="contactEmail">Contact email of the user</param>
        /// <param name="isProdUser">Is the user required for prod</param>
        /// <returns>New user details</returns>
        Task<NewUserResponse> CreateNewUserInAAD(string firstName, string lastName, string contactEmail, bool isProdUser);

        /// <summary>Deletes a user by contact email</summary>
        /// <param name="contactEmail">Contact email of the user</param>
        /// <returns></returns>
        Task DeleteUserInAAD(string contactEmail);

        /// <summary>Get an AD user profile by contact email</summary>
        /// <param name="contactEmail">Contact email of the user</param>
        /// <returns>User profile of the user</returns>
        Task<UserProfile> GetADUserProfile(string contactEmail);

        /// <summary>Checks that the user has the required groups and adds any missing ones</summary>
        /// <param name="user">The test api user profile</param>
        /// <param name="adUserProfile">The AD user profile</param>
        /// <returns>A count of the number of groups the user now has</returns>
        Task<int> AddGroupsToUserIfRequired(User user, UserProfile adUserProfile);
    }

    public class UserApiService : IUserApiService
    {
        protected const int POLLY_RETRIES = 4;
        private readonly IUserApiClient _userApiClient;
        private readonly UserGroupsConfiguration _userGroups;

        public UserApiService(IUserApiClient userApiClient, IOptions<UserGroupsConfiguration> userGroupsConfiguration)
        {
            _userApiClient = userApiClient;
            _userGroups = userGroupsConfiguration.Value;
            ValidateGroupsAreSet(userGroupsConfiguration.Value);
        }

        private static void ValidateGroupsAreSet(UserGroupsConfiguration values)
        {
            values.GetType().GetProperties()
                .Where(pi => pi.PropertyType == typeof(string))
                .Select(pi => (string)pi.GetValue(values))
                .Any(string.IsNullOrEmpty)
                .Should().BeFalse("All values are set");
        }

        public async Task<bool> CheckUserExistsInAAD(string contactEmail)
        {
            var policy = Policy
                .Handle<UserApiException>(ex => ex.StatusCode.Equals(HttpStatusCode.InternalServerError))
                .WaitAndRetryAsync(POLLY_RETRIES, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            try
            {
                await policy.ExecuteAsync(async () => await _userApiClient.GetUserByEmailAsync(contactEmail));
            }
            catch (UserApiException e)
            {
                if (e.StatusCode == (int) HttpStatusCode.NotFound) return false;

                if (e.StatusCode == (int) HttpStatusCode.InternalServerError) throw;
            }

            return true;
        }

        public async Task<NewUserResponse> CreateNewUserInAAD(string firstName, string lastName, string contactEmail, bool isProdUser)
        {
            const string BLANK = " ";

            var createUserRequest = new CreateUserRequest
            {
                First_name = firstName.Replace(BLANK, string.Empty),
                Last_name = lastName.Replace(BLANK, string.Empty),
                Recovery_email = contactEmail,
                Is_test_user = true
            };

            return await _userApiClient.CreateUserAsync(createUserRequest);
        }

        public async Task DeleteUserInAAD(string contactEmail)
        {
            try
            {
                await _userApiClient.DeleteUserAsync(contactEmail);
            }
            catch (UserApiException e)
            {
                if (e.StatusCode == (int) HttpStatusCode.NotFound) throw;

                if (e.StatusCode == (int) HttpStatusCode.InternalServerError) throw;
            }
        }

        public async Task<UserProfile> GetADUserProfile(string contactEmail)
        {
            var policy = Policy
                .Handle<UserApiException>()
                .WaitAndRetryAsync(POLLY_RETRIES, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            return await policy.ExecuteAsync(async () => await _userApiClient.GetUserByEmailAsync(contactEmail));
        }

        public async Task<int> AddGroupsToUserIfRequired(User user, UserProfile adUserProfile)
        {
            var policy = Policy
                .Handle<UserApiException>()
                .WaitAndRetryAsync(POLLY_RETRIES, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var existingGroups = await policy.ExecuteAsync(async () => await _userApiClient.GetGroupsForUserAsync(adUserProfile.User_id));
            var requiredGroups = GetRequiredGroups(user);

            foreach (var requiredGroup in requiredGroups.Where(requiredGroup => !existingGroups.Any(x => x.Display_name.Equals(requiredGroup, StringComparison.CurrentCultureIgnoreCase))))
            {
                await AddUserToGroup(adUserProfile.User_id, requiredGroup);
            }

            return requiredGroups.Count;
        }

        private List<string> GetRequiredGroups(User user)
        {
            var userGroupStrategies = new UserGroups().GetStrategies();
            var groups = userGroupStrategies[user.UserType].GetGroups(_userGroups);

            if (!user.IsProdUser && user.UserType != UserType.Judge) groups.Add(_userGroups.TestAccountGroup);

            if (IsPerformanceTestUser(user.FirstName)) groups.Add(_userGroups.PerformanceTestAccountGroup);

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
                User_id = adUserId,
                Group_name = group
            };

            await PollToAddUserToGroup(request);
        }

        private async Task PollToAddUserToGroup(AddUserToGroupRequest request)
        {
            var policy = Policy
                .Handle<UserApiException>(ex => ex.StatusCode.Equals(HttpStatusCode.NotFound))
                .WaitAndRetryAsync(POLLY_RETRIES, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            
            await policy.ExecuteAsync(async () => await _userApiClient.AddUserToGroupAsync(request));
        }
    }
}