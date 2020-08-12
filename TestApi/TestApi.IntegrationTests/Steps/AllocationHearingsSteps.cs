using System;
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
using TestApi.Contract.Responses;
using TestApi.Domain.Enums;
using TestApi.Domain.Helpers;
using TestApi.IntegrationTests.Configuration;
using TestApi.IntegrationTests.Helpers;
using TestApi.Mappings;

namespace TestApi.IntegrationTests.Steps
{
    [Binding]
    public class AllocationHearingsSteps : BaseSteps
    {
        private readonly TestContext _context;

        public AllocationHearingsSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have the following list of users (.*)")]
        public void GivenIHaveTheFollowingListOfUsers(string usersString)
        {
            var userTypes = ConvertStringToList(usersString);

            _context.Test.AllocationRequest = new AllocateUsersBuilder()
                .WithUserTypes(userTypes)
                .ForApplication(Application.TestApi)
                .Build();
        }

        [Given(@"I have an allocate users request")]
        public void GivenIHaveAnAllocateUsersRequest()
        {
            _context.Uri = ApiUriFactory.AllocationEndpoints.AllocateUsers;
            _context.HttpMethod = HttpMethod.Put;
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(_context.Test.AllocationRequest);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a create hearing request for the previously allocated users")]
        public void GivenIHaveACreateHearingRequestForThePreviouslyAllocatedUsers()
        {
            _context.Test.Users = UserDetailsResponseToUserMapper.Map(_context.Test.UserResponses);
            _context.Test.CreateHearingRequest = new HearingBuilder(_context.Test.Users).BuildRequest();
            _context.Uri = ApiUriFactory.HearingEndpoints.CreateHearing;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(_context.Test.CreateHearingRequest);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Then(@"the user details for the (.*) allocations should be retrieved")]
        public async Task ThenTheUserDetailsForTheAllocationsShouldBeRetrieved(string usersString)
        {
            var userTypes = ConvertStringToList(usersString);

            var responses = await Response.GetResponses<List<UserDetailsResponse>>(_context.Response.Content);
            responses.Should().NotBeNull();
            responses.Count.Should().BeGreaterThan(0);

            for (var i = 0; i < userTypes.Count; i++)
            {
                responses[i].Application.Should().Be(Application.TestApi);
                responses[i].ContactEmail.Should().Contain(userTypes[i].ToString().ToLower());
                responses[i].DisplayName.Should().Contain(userTypes[i].ToString());
                responses[i].FirstName.Should().Contain("TA");
                responses[i].Id.Should().NotBeEmpty();
                responses[i].LastName.Should().Contain(userTypes[i].ToString());
                responses[i].Number.Should().BeGreaterThan(0);
                responses[i].UserType.Should().Be(userTypes[i]);
                responses[i].Username.Should().Contain(userTypes[i].ToString().ToLower());
            }

            _context.Test.UserResponses = responses;
        }

        [Then(@"the users should be allocated")]
        public async Task ThenTheUsersShouldBeAllocated()
        {
            foreach (var user in _context.Test.UserResponses)
            {
                var allocation = await _context.TestDataManager.GetAllocationByUserId(user.Id);
                allocation.Should().NotBeNull();
                allocation.Allocated.Should().BeTrue();
                allocation.ExpiresAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(9));
                allocation.ExpiresAt.Should().BeBefore(DateTime.UtcNow.AddMinutes(10));
            }
        }

        private static List<UserType> ConvertStringToList(string userTypes)
        {
            return userTypes.Split(", ").Select(UserTypeName.FromString).ToList();
        }
    }
}
