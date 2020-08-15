using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Contract.Responses;
using TestApi.DAL.Queries;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.UnitTests.Controllers.Allocations
{
    public class AllocateSingleUserTests : HearingsControllerTestBase
    {
        [TestCase(UserType.Judge)]
        [TestCase(UserType.Individual)]
        [TestCase(UserType.Representative)]
        [TestCase(UserType.Observer)]
        [TestCase(UserType.PanelMember)]
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.VideoHearingsOfficer)]
        public async Task Should_return_allocated_user(UserType userType)
        {
            var user = CreateUser(userType);
            CreateAllocation(user);

            QueryHandler
                .Setup(
                    x => x.Handle<GetAllocatedUserByUserTypeQuery, User>(It.IsAny<GetAllocatedUserByUserTypeQuery>()))
                .ReturnsAsync(user);

            var response = await Controller.AllocateSingleUserAsync(user.UserType, user.Application);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var userDetailsResponse = (UserDetailsResponse) result.Value;
            userDetailsResponse.Should().BeEquivalentTo(user);
        }
    }
}