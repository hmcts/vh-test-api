﻿using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Contract.Dtos;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.DAL.Queries;
using TestApi.Contract.Enums;

namespace TestApi.UnitTests.Controllers.Allocations
{
    public class AllocateUsersTests : HearingsControllerTestBase
    {
        [Test]
        public void Should_return_allocated_users()
        {
            const UserType JUDGE_USER_TYPE = UserType.Judge;
            const UserType INDIVIDUAL_USER_TYPE = UserType.Individual;
            const UserType REPRESENTATIVE_USER_TYPE = UserType.Representative;
            const UserType CASE_ADMIN_USER_TYPE = UserType.CaseAdmin;
            const UserType STAFF_MEMBER_TYPE = UserType.StaffMember;

            var judgeUser = CreateUser(JUDGE_USER_TYPE);
            CreateAllocation(judgeUser);

            var individualUser = CreateUser(INDIVIDUAL_USER_TYPE);
            CreateAllocation(individualUser);

            var representativeUser = CreateUser(REPRESENTATIVE_USER_TYPE);
            CreateAllocation(representativeUser);

            var caseAdminUser = CreateUser(CASE_ADMIN_USER_TYPE);
            CreateAllocation(caseAdminUser);

            var staffMember = CreateUser(STAFF_MEMBER_TYPE);
            CreateAllocation(staffMember);

            var request = new AllocateUsersRequest
            {
                Application = Application.TestApi,
                UserTypes = new List<UserType>
                    {JUDGE_USER_TYPE, INDIVIDUAL_USER_TYPE, REPRESENTATIVE_USER_TYPE, CASE_ADMIN_USER_TYPE, STAFF_MEMBER_TYPE}
            };

            QueryHandler
                .SetupSequence(x =>
                    x.Handle<GetAllocatedUserByUserTypeQuery, UserDto>(It.IsAny<GetAllocatedUserByUserTypeQuery>()))
                .ReturnsAsync(judgeUser)
                .ReturnsAsync(individualUser)
                .ReturnsAsync(representativeUser)
                .ReturnsAsync(caseAdminUser)
                .ReturnsAsync(staffMember);

            var response = Controller.AllocateUsers(request);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var userDetailsResponse = (List<UserDetailsResponse>) result.Value;
            userDetailsResponse.Count.Should().Be(5);

            userDetailsResponse[0].UserType.Should().Be(JUDGE_USER_TYPE);
            userDetailsResponse[0].Should().BeEquivalentTo(judgeUser);

            userDetailsResponse[1].UserType.Should().Be(INDIVIDUAL_USER_TYPE);
            userDetailsResponse[1].Should().BeEquivalentTo(individualUser);

            userDetailsResponse[2].UserType.Should().Be(REPRESENTATIVE_USER_TYPE);
            userDetailsResponse[2].Should().BeEquivalentTo(representativeUser);

            userDetailsResponse[3].UserType.Should().Be(CASE_ADMIN_USER_TYPE);
            userDetailsResponse[3].Should().BeEquivalentTo(caseAdminUser);

            userDetailsResponse[4].UserType.Should().Be(STAFF_MEMBER_TYPE);
            userDetailsResponse[4].Should().BeEquivalentTo(staffMember);
        }
    }
}