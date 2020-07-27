using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Contract.Responses;
using TestApi.DAL.Queries;
using TestApi.Domain;

namespace TestApi.UnitTests.Controllers.Allocations
{
    public class GetAllocationDetailsByUserIdTests : AllocationControllerTestBase
    {
        [Test]
        public async Task Should_return_ok_for_existing_user_id()
        {
            var user = CreateUser();
            var allocation = new Allocation(user);

            QueryHandlerMock
                .Setup(x => x.Handle<GetUserByIdQuery, User>(It.IsAny<GetUserByIdQuery>()))
                .ReturnsAsync(user);

            QueryHandlerMock
                .Setup(x => x.Handle<GetAllocationByUserIdQuery, Allocation>(It.IsAny<GetAllocationByUserIdQuery>()))
                .ReturnsAsync(allocation);

            var result = await Controller.GetAllocationDetailsByUserIdAsync(user.Id);
            result.Should().NotBeNull();

            var objectResult = (OkObjectResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var allocationDetailsResponse = (AllocationDetailsResponse) objectResult.Value;
            allocationDetailsResponse.Should().NotBeNull();
            allocationDetailsResponse.Allocated.Should().BeFalse();
            allocationDetailsResponse.ExpiresAt.Should().BeNull();
            allocationDetailsResponse.UserId.Should().Be(user.Id);
            allocationDetailsResponse.Username.Should().Be(user.Username);

            QueryHandlerMock.Verify(
                x => x.Handle<GetAllocationByUserIdQuery, Allocation>(It.IsAny<GetAllocationByUserIdQuery>()),
                Times.Once);
        }

        [Test]
        public async Task Should_return_not_found_for_nonexistent_user()
        {
            var result = await Controller.GetAllocationDetailsByUserIdAsync(Guid.NewGuid());
            result.Should().NotBeNull();
            var objectResult = (NotFoundResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }
    }
}
