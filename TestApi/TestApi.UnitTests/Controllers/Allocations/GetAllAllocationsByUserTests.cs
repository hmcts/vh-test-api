using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Contract.Responses;
using TestApi.DAL.Queries;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.UnitTests.Controllers.Allocations
{
    public class GetAllAllocationsByUserTests : HearingsControllerTestBase
    {
        [Test]
        public async Task Should_return_all_allocated_users()
        {
            const string ALLOCATED_BY = EmailData.TEST_WEB_MANUAL_USER;
            var judgeUser = CreateUser(UserType.Judge);
            var judgeAllocation = CreateAllocation(judgeUser);
            judgeAllocation.AllocatedBy = ALLOCATED_BY;
            var vhoUser = CreateUser(UserType.VideoHearingsOfficer);
            var vhoAllocation = CreateAllocation(vhoUser);
            vhoAllocation.AllocatedBy = ALLOCATED_BY;
            var allocations = new List<Allocation> {judgeAllocation, vhoAllocation};

            QueryHandler
                .Setup(
                    x => x.Handle<GetAllAllocationsForAUserQuery, List<Allocation>>(
                        It.IsAny<GetAllAllocationsForAUserQuery>()))
                .ReturnsAsync(allocations);

            var response = await Controller.GetAllocatedUsersAsync(ALLOCATED_BY);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var allocationDetailsResponse = (List<AllocationDetailsResponse>) result.Value;
            allocationDetailsResponse.Should().BeEquivalentTo(allocations, options => options.Excluding(x => x.User));
        }

        [Test]
        public async Task Should_return_empty_list_if_no_users_exist()
        {
            const string ALLOCATED_BY = EmailData.TEST_WEB_MANUAL_USER;

            QueryHandler
                .Setup(
                    x => x.Handle<GetAllAllocationsForAUserQuery, List<Allocation>>(
                        It.IsAny<GetAllAllocationsForAUserQuery>()))
                .ReturnsAsync(new List<Allocation>());

            var response = await Controller.GetAllocatedUsersAsync(ALLOCATED_BY);
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var allocationDetailsResponse = (List<AllocationDetailsResponse>)result.Value;
            allocationDetailsResponse.Should().BeEmpty();
        }
    }
}