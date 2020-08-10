﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestApi.Common.Builders;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.Domain.Enums;
using TestApi.IntegrationTests.Configuration;
using TestApi.IntegrationTests.Helpers;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.IntegrationTests.Steps
{
    [Binding]
    public class UserSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly CommonSteps _commonSteps;
        private CreateADUserRequest _createAdUserRequest;

        public UserSteps(TestContext context, CommonSteps commonSteps)
        {
            _context = context;
            _commonSteps = commonSteps;
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

        [Given(@"I have a valid create AD user request")]
        public void GivenIHaveAValidCreateAadUserRequest()
        {
            var userRequest = new UserBuilder(_context.Config.UsernameStem, 1)
                .WithUserType(UserType.Individual)
                .ForApplication(Application.TestApi)
                .BuildRequest();

            _createAdUserRequest = new ADUserBuilder(userRequest).BuildRequest();

            _context.Uri = ApiUriFactory.UserEndpoints.CreateAADUser;
            _context.HttpMethod = HttpMethod.Post;

            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(_createAdUserRequest);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [When(@"I send the create user request twice")]
        public async Task WhenISendTheCreateUserRequestTwice()
        {
            await SendCreateRequestTwice();
            _context.Test.User = await _context.TestDataManager.SeedUser();
            await SendCreateRequestTwice();
        }

        private async Task SendCreateRequestTwice()
        {
            await _commonSteps.WhenISendTheRequestToTheEndpoint();
            _context.Response.StatusCode.Should().Be(HttpStatusCode.Created);
            var response = await Response.GetResponses<UserDetailsResponse>(_context.Response.Content);
            _context.Test.UserResponses.Add(response);
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

        [Then(@"the iterated number should be retrieved")]
        public async Task ThenTheIteratedNumberShouldBeRetrieved()
        {
            var response = await Response.GetResponses<IteratedUserNumberResponse>(_context.Response.Content);
            response.Should().NotBeNull();
            response.Number.Should().BeGreaterThan(0);
        }

        [Then(@"the details of the new AD user are retrieved")]
        public async Task ThenTheDetailsOfTheNewADUserAreRetrieved()
        {
            var response = await Response.GetResponses<NewUserResponse>(_context.Response.Content);
            response.Should().NotBeNull();
            response.User_id.Should().NotBeNullOrWhiteSpace();
            response.One_time_password.Should().NotBeNullOrWhiteSpace();
            response.Username.Should().Be(_createAdUserRequest.Username);
        }
    }
}
