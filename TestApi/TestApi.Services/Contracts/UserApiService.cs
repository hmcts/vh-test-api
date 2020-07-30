using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TestApi.Common.Configuration;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Domain.Helpers;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Exceptions;
using TestApi.Services.Helpers;
using CreateUserRequest = TestApi.Services.Clients.UserApiClient.CreateUserRequest;

namespace TestApi.Services.Contracts
{
    public interface IUserApiService
    {
        /// <summary>
        /// Checks the health of the user api
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckHealth();

        /// <summary>
        /// Checks if a user already exists based on their contact email
        /// </summary>
        /// <param name="contactEmail"></param>
        /// <returns></returns>
        Task<bool> CheckUserExistsInAAD(string contactEmail);

        /// <summary>
        /// Creates a user based on the user information
        /// </summary>
        /// <param name="adUser"></param>
        /// <returns></returns>
        Task<NewUserResponse> CreateNewUserInAAD(ADUser adUser);


    }

    public class UserApiService : IUserApiService
    {
        private readonly IUserApiClient _userApiClient;
        private readonly UserGroupsConfiguration _userGroups;

        public UserApiService(IUserApiClient userApiClient, IOptions<UserGroupsConfiguration> userGroupsConfiguration)
        {
            _userApiClient = userApiClient;
            _userGroups = userGroupsConfiguration.Value;
        }

        public async Task<bool> CheckHealth()
        {
            try
            {
                await _userApiClient.CheckServiceHealthAsync();
                var response = await _userApiClient.GetGroupByNameAsync(_userGroups.TestAccountGroup);
            }
            catch (UserApiException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    return true;
                }

                if (e.StatusCode == (int) HttpStatusCode.InternalServerError)
                {
                    throw;
                }

                return false;
            }

            return true;
        }

        public async Task<bool> CheckUserExistsInAAD(string contactEmail)
        {
            try
            {
                await _userApiClient.GetUserByEmailAsync(contactEmail);
            }
            catch (UserApiException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    return false;
                }

                if (e.StatusCode == (int)HttpStatusCode.InternalServerError)
                {
                    throw;
                }
            }

            return true;
        }

        public async Task<NewUserResponse> CreateNewUserInAAD(ADUser adUser)
        {
            var userType = GetUserType.FromUserLastName(adUser.LastName);

            const string BLANK = " ";

            var createUserRequest = new CreateUserRequest
            {
                First_name = adUser.FirstName.Replace(BLANK, string.Empty),
                Last_name = adUser.LastName.Replace(BLANK, string.Empty),
                Recovery_email = adUser.ContactEmail
            };

            if (adUser.ContactEmail != createUserRequest.Recovery_email)
            {
                throw new UserDetailsMismatchException(adUser.ContactEmail, createUserRequest.Recovery_email);
            }

            var newUserResponse = await _userApiClient.CreateUserAsync(createUserRequest);

            await AddUserToGroups(newUserResponse.User_id, userType);

            return newUserResponse;
        }

        private async Task AddUserToGroups(string userId, UserType userType, bool isPerformanceTestUser = false, bool isProdUser = false)
        {
            var userGroupStrategies = new UserGroups().GetStrategies();
            var groups = userGroupStrategies[userType].GetGroups(_userGroups);

            if (!isProdUser)
            {
                groups.Add(_userGroups.TestAccountGroup);
            }

            if (isPerformanceTestUser)
            {
                groups.Add(_userGroups.PerformanceTestAccountGroup);
            }

            foreach (var group in groups)
            {
                var request = new AddUserToGroupRequest
                {
                    User_id = userId,
                    Group_name = group
                };

                await _userApiClient.AddUserToGroupAsync(request);
            }
        }
    }
}
