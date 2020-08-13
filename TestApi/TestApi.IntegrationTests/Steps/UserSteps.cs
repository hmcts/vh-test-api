using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestApi.Contract.Responses;
using TestApi.Domain.Enums;
using TestApi.IntegrationTests.Configuration;
using TestApi.IntegrationTests.Helpers;

namespace TestApi.IntegrationTests.Steps
{
    [Binding]
    public class UserSteps : BaseSteps
    {
        private readonly TestContext _context;

        public UserSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a valid get user details by username request")]
        public void GivenIHaveAValidGetUserDetailsByUsernameRequest()
        {
            _context.Uri = ApiUriFactory.UserEndpoints.GetUserByUsername(_context.Test.User.Username);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get user details by username request with a nonexistent username")]
        public void GivenIHaveAGetUserDetailsByUsernameRequestWithANonexistentUsername()
        {
            _context.Uri = ApiUriFactory.UserEndpoints.GetUserByUsername("MadeUpUsername@email.com");
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a valid delete AD user request")]
        public void GivenIHaveAValidDeleteADUserRequest()
        {
            _context.Uri = ApiUriFactory.UserEndpoints.DeleteAADUser(_context.Test.User.ContactEmail);
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a delete AD user request for a nonexistent user")]
        public void GivenIHaveADeleteADUserRequestForANonexistentUser()
        {
            const string EMAIL = "made_up_email@email.com";
            _context.Uri = ApiUriFactory.UserEndpoints.DeleteAADUser(EMAIL);
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Then(@"the user numbers should be incremented")]
        public void ThenTheNumbersShouldBeIncremented()
        {
            var firstUserNumber = _context.Test.UserResponses.First().Number;
            var secondUserNumber = _context.Test.UserResponses.Last().Number;
            firstUserNumber.Should().Be(secondUserNumber - 1);
        }

        [Then(@"the response contains the new user details")]
        public async Task ThenTheResponseContainsTheNewUserDetails()
        {
            var response = await Response.GetResponses<UserDetailsResponse>(_context.Response.Content);
            response.Should().NotBeNull();
            response.Application.Should().Be(_context.Test.User.Application);
            response.ContactEmail.Should().Be(_context.Test.User.ContactEmail);
            response.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            response.DisplayName.Should().Be(_context.Test.User.DisplayName);
            response.FirstName.Should().Be(_context.Test.User.FirstName);
            response.Id.Should().NotBeEmpty();
            response.LastName.Should().Be(_context.Test.User.LastName);
            response.Number.Should().Be(_context.Test.User.Number);
            response.UserType.Should().Be(_context.Test.User.UserType);
            response.Username.Should().Be(_context.Test.User.Username);
        }

        [Then(@"the user details for the newly created (.*) user during allocation should be retrieved")]
        public async Task ThenTheUserDetailsShouldBeRetrieved(UserType userType)
        {
            var response = await Response.GetResponses<UserDetailsResponse>(_context.Response.Content);
            response.Should().NotBeNull();
            response.Application.Should().Be(Application.TestApi);
            response.ContactEmail.Should().Contain(userType.ToString().ToLower());
            response.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
            response.DisplayName.Should().Contain(userType.ToString());
            response.FirstName.Should().Contain("TA");
            response.Id.Should().NotBeEmpty();
            response.LastName.Should().Contain(userType.ToString());
            response.Number.Should().BeGreaterThan(0);
            response.UserType.Should().Be(userType);
            response.Username.Should().Contain(userType.ToString().ToLower());
            _context.Test.UserDetailsResponse = response;
        }

        [Then(@"the user details should be retrieved")]
        public async Task ThenTheUserDetailsShouldBeRetrieved()
        {
            var response = await Response.GetResponses<UserDetailsResponse>(_context.Response.Content);
            response.Should().NotBeNull();
            response.Application.Should().Be(_context.Test.User.Application);
            response.ContactEmail.Should().Be(_context.Test.User.ContactEmail);
            response.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            response.DisplayName.Should().Be(_context.Test.User.DisplayName);
            response.FirstName.Should().Be(_context.Test.User.FirstName);
            response.Id.Should().NotBeEmpty();
            response.LastName.Should().Be(_context.Test.User.LastName);
            response.Number.Should().Be(_context.Test.User.Number);
            response.UserType.Should().Be(_context.Test.User.UserType);
            response.Username.Should().Be(_context.Test.User.Username);
            _context.Test.UserDetailsResponse = response;
        }

        [Then(@"a list of user details for the given user type and application should be retrieved")]
        public async Task ThenAListOfUserDetailsShouldBeRetrieved()
        {
            var users = await Response.GetResponses<List<UserDetailsResponse>>(_context.Response.Content);
            users.Count.Should().BeGreaterOrEqualTo(2);
            users.All(x => x.UserType == UserType.Judge && x.Application == Application.TestApi).Should().BeTrue();
            users.Any(x => string.Equals(x.Username, _context.Test.Users.First().Username, StringComparison.CurrentCultureIgnoreCase)).Should().BeTrue();
            users.Any(x => string.Equals(x.Username, _context.Test.Users[1].Username, StringComparison.CurrentCultureIgnoreCase)).Should().BeTrue();
            users.Any(x => string.Equals(x.Username, _context.Test.Users.Last().Username, StringComparison.CurrentCultureIgnoreCase)).Should().BeFalse();
        }
    }
}
