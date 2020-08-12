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
    public class GetAllocationsByUserTypesTests : HearingsControllerTestBase
    {
        [Test]
        public async Task Should_return_allocated_users()
        {
            const UserType FIRST_USER_TYPE = UserType.Judge;
            const UserType SECOND_USER_TYPE = UserType.Individual;
            const UserType THIRD_USER_TYPE = UserType.Representative;

            var firstUser = CreateUser(FIRST_USER_TYPE);
            CreateAllocation(firstUser);

            var secondUser = CreateUser(SECOND_USER_TYPE);
            CreateAllocation(secondUser);

            var thirdUser = CreateUser(THIRD_USER_TYPE);
            CreateAllocation(thirdUser);

            var request = new AllocateUsersRequest()
            {
                Application = Application.TestApi,
                UserTypes = new List<UserType>() { FIRST_USER_TYPE, SECOND_USER_TYPE, THIRD_USER_TYPE }
            };

            QueryHandler
                .SetupSequence(x => x.Handle<GetAllocatedUserByUserTypeQuery, User>(It.IsAny<GetAllocatedUserByUserTypeQuery>()))
                .ReturnsAsync(firstUser)
                .ReturnsAsync(secondUser)
                .ReturnsAsync(thirdUser);

            var response = await Controller.AllocateUsersAsync(request);
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var userDetailsResponse = (List<UserDetailsResponse>)result.Value;
            userDetailsResponse.Count.Should().Be(3);

            userDetailsResponse[0].UserType.Should().Be(FIRST_USER_TYPE);
            userDetailsResponse[0].Should().BeEquivalentTo(firstUser);

            userDetailsResponse[1].UserType.Should().Be(SECOND_USER_TYPE);
            userDetailsResponse[1].Should().BeEquivalentTo(secondUser);

            userDetailsResponse[2].UserType.Should().Be(THIRD_USER_TYPE);
            userDetailsResponse[2].Should().BeEquivalentTo(thirdUser);
        }
    }
}
