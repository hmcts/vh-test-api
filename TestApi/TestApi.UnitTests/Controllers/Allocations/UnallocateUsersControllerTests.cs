using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.DAL.Queries;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.UnitTests.Controllers.Allocations
{
    public class UnallocateUsersControllerTests : HearingsControllerTestBase
    {
        [Test]
        public async Task Should_unallocate_users()
        {
            const UserType FIRST_USER_TYPE = UserType.Judge;
            const UserType SECOND_USER_TYPE = UserType.Individual;
            const UserType THIRD_USER_TYPE = UserType.Representative;

            var firstUser = CreateUser(FIRST_USER_TYPE);
            var firstAllocation = CreateAllocation(firstUser);
            var firstUnallocation = Unallocate(firstAllocation);

            var secondUser = CreateUser(SECOND_USER_TYPE);
            var secondAllocation = CreateAllocation(secondUser);
            var secondUnallocation = Unallocate(secondAllocation);

            var thirdUser = CreateUser(THIRD_USER_TYPE);
            var thirdAllocation = CreateAllocation(thirdUser);
            var thirdUnallocation = Unallocate(thirdAllocation);

            var request = new UnallocateUsersRequest
            {
                Usernames = new List<string> {firstUser.Username, secondUser.Username, thirdUser.Username}
            };

            QueryHandler
                .SetupSequence(x => x.Handle<GetUserByUsernameQuery, User>(It.IsAny<GetUserByUsernameQuery>()))
                .ReturnsAsync(firstUser)
                .ReturnsAsync(secondUser)
                .ReturnsAsync(thirdUser);

            QueryHandler
                .SetupSequence(x =>
                    x.Handle<GetAllocationByUsernameQuery, Allocation>(It.IsAny<GetAllocationByUsernameQuery>()))
                .ReturnsAsync(firstAllocation)
                .ReturnsAsync(firstUnallocation)
                .ReturnsAsync(secondAllocation)
                .ReturnsAsync(secondUnallocation)
                .ReturnsAsync(thirdAllocation)
                .ReturnsAsync(thirdUnallocation);

            var response = await Controller.UnallocateUsersByUsernameAsync(request);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var allocationDetailsResponses = (List<AllocationDetailsResponse>) result.Value;
            allocationDetailsResponses.Count.Should().Be(3);
            allocationDetailsResponses[0].Should().BeEquivalentTo(firstAllocation, options => options.Excluding(x => x.User));
            allocationDetailsResponses[1].Should().BeEquivalentTo(secondAllocation, options => options.Excluding(x => x.User));
            allocationDetailsResponses[2].Should().BeEquivalentTo(thirdAllocation, options => options.Excluding(x => x.User));
        }

        [Test]
        public async Task Should_return_not_found_if_user_does_not_exist()
        {
            var request = new UnallocateUsersRequest
            {
                Usernames = new List<string> {"does_not_exist@hmcts.net"}
            };

            var response = await Controller.UnallocateUsersByUsernameAsync(request);
            response.Should().NotBeNull();

            var result = (NotFoundResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_bad_request_if_allocation_does_not_exist()
        {
            var user = CreateUser(UserType.Judge);

            var request = new UnallocateUsersRequest
            {
                Usernames = new List<string> {user.Username}
            };

            QueryHandler
                .Setup(x => x.Handle<GetUserByUsernameQuery, User>(It.IsAny<GetUserByUsernameQuery>()))
                .ReturnsAsync(user);

            var response = await Controller.UnallocateUsersByUsernameAsync(request);
            response.Should().NotBeNull();

            var result = (BadRequestObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            result.Value.Should().Be($"No allocation exists for user with username {user.Username}");
        }
    }
}