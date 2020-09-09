using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Polly;
using TestApi.Common.Configuration;
using TestApi.Common.Data;
using TestApi.Domain.Enums;
using TestApi.Domain.Helpers;
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
    }

    public class UserApiService : IUserApiService
    {
        protected const int ADD_TO_USER_GROUP_RETRIES = 4;
        private readonly IUserApiClient _userApiClient;
        private readonly UserGroupsConfiguration _userGroups;

        public UserApiService(IUserApiClient userApiClient, IOptions<UserGroupsConfiguration> userGroupsConfiguration)
        {
            _userApiClient = userApiClient;
            _userGroups = userGroupsConfiguration.Value;
        }

        public async Task<bool> CheckUserExistsInAAD(string contactEmail)
        {
            try
            {
                await _userApiClient.GetUserByEmailAsync(contactEmail);
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

            var newUserResponse = await _userApiClient.CreateUserAsync(createUserRequest);

            var userType = GetUserType.FromUserLastName(lastName);

            await AddUserToGroups(newUserResponse.User_id, userType, IsPerformanceTestUser(firstName), isProdUser);

            return newUserResponse;
        }

        private static bool IsPerformanceTestUser(string firstName)
        {
            return firstName.Contains(UserData.PERFORMANCE_FIRST_NAME_PREFIX);
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

        private async Task AddUserToGroups(string userId, UserType userType, bool isPerformanceTestUser = false,
            bool isProdUser = false)
        {
            var userGroupStrategies = new UserGroups().GetStrategies();
            var groups = userGroupStrategies[userType].GetGroups(_userGroups);

            if (!isProdUser && userType != UserType.Judge) groups.Add(_userGroups.TestAccountGroup);

            if (isPerformanceTestUser) groups.Add(_userGroups.PerformanceTestAccountGroup);

            foreach (var group in groups)
            {
                var request = new AddUserToGroupRequest
                {
                    User_id = userId,
                    Group_name = group
                };

                await PollToAddUserToGroup(request);
            }
        }

        private async Task PollToAddUserToGroup(AddUserToGroupRequest request)
        {
            var policy = Policy
                .Handle<UserApiException>(ex => ex.StatusCode.Equals(HttpStatusCode.NotFound))
                .WaitAndRetryAsync(ADD_TO_USER_GROUP_RETRIES, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            try
            {
                await policy.ExecuteAsync(async () => await _userApiClient.AddUserToGroupAsync(request));
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {ADD_TO_USER_GROUP_RETRIES ^ 2} seconds.");
            }
        }
    }
}