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
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.Domain.Enums;
using TestApi.IntegrationTests.Configuration;
using TestApi.IntegrationTests.Helpers;

namespace TestApi.IntegrationTests.Steps
{
    [Binding]
    public class AllocationSteps : BaseSteps
    {
        private readonly TestContext _context;

        public AllocationSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a Allocate user by user type (.*) and application request")]
        public void GivenIHaveAAllocateUserByUserTypeAndApplicationRequest(UserType userType)
        {
            _context.Uri = ApiUriFactory.AllocationEndpoints.AllocateByUserTypeAndApplication(userType, Application.TestApi);
            _context.HttpMethod = HttpMethod.Put;
        }

        [Given(@"I have a valid unallocate users by username request")]
        public void GivenIHaveAValidUnallocateUsersByUsernameRequest()
        {
            var usernames = _context.Test.Users.Select(user => user.Username).ToList();

            var request = new UnallocateUsersRequest()
            {
                Usernames = usernames
            };

            _context.Uri = ApiUriFactory.AllocationEndpoints.UnallocateUsers;
            _context.HttpMethod = HttpMethod.Put;

            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a valid unallocate users by username request for a nonexistent user")]
        public void GivenIHaveAValidUnallocateUsersByUsernameRequestForANonexistentUser()
        {
            var usernames = new List<string>(){"MadeUpUsername@email.com"};

            var request = new UnallocateUsersRequest()
            {
                Usernames = usernames
            };

            _context.Uri = ApiUriFactory.AllocationEndpoints.UnallocateUsers;
            _context.HttpMethod = HttpMethod.Put;

            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Then(@"the user should be allocated")]
        public async Task ThenTheUserShouldBeAllocated()
        {
            var allocation = await _context.TestDataManager.GetAllocationByUserId(_context.Test.UserDetailsResponse.Id);
            allocation.Should().NotBeNull();
            allocation.Allocated.Should().BeTrue();
            allocation.ExpiresAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(9));
            allocation.ExpiresAt.Should().BeBefore(DateTime.UtcNow.AddMinutes(10));
        }

        [Then(@"the response contains the allocation details")]
        public async Task ThenTheResponseContainsTheNewAllocationDetails()
        {
            var response = await Response.GetResponses<AllocationDetailsResponse>(_context.Response.Content);
            response.Should().NotBeNull();
            response.Id.Should().NotBeEmpty();
            response.Allocated.Should().BeFalse();
            response.ExpiresAt.Should().BeNull();
            response.UserId.Should().Be(_context.Test.User.Id);
            response.Username.Should().Contain(_context.Test.User.Username);
        }

        [Then(@"the response contains the allocation details for the (.*)")]
        public async Task ThenTheResponseContainsTheNewAllocationDetailsForTheUserType(UserType userType)
        {
            var response = await Response.GetResponses<AllocationDetailsResponse>(_context.Response.Content);
            response.Should().NotBeNull();
            response.Id.Should().NotBeEmpty();
            response.Allocated.Should().BeFalse();
            response.ExpiresAt.Should().BeNull();
            response.UserId.Should().NotBeEmpty();
            response.Username.Should().Contain(userType.ToString());
        }

        [Then(@"a list of user allocation details should be retrieved for the (.*) users")]
        public async Task ThenAListOfUserAllocationDetailsShouldBeRetrieved(string allocatedText)
        {
            var allocated = allocatedText.ToLower().Equals("allocated");

            var response = await Response.GetResponses<List<AllocationDetailsResponse>>(_context.Response.Content);
            response.Count.Should().BeGreaterThan(0);

            foreach (var allocationDetails in response)
            {
                _context.Test.Allocations.Any(x => x.Id == allocationDetails.Id).Should().BeTrue();
                _context.Test.Users.Any(x => x.Id == allocationDetails.UserId).Should().BeTrue();
                _context.Test.Users.Any(x => x.Username == allocationDetails.Username).Should().BeTrue();
                
                allocationDetails.Allocated.Should().Be(allocated);

                if (allocated)
                {
                    allocationDetails.ExpiresAt.Should().NotBeNull();
                }
                else
                {
                    allocationDetails.ExpiresAt.Should().BeNull();
                }
            }
        }
    }
}
