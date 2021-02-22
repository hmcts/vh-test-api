using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
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
        [TestCase(UserType.Tester)]
        [TestCase(UserType.Witness)]
        [TestCase(UserType.Interpreter)]
        public void Should_return_allocated_user(UserType userType)
        {
            var user = CreateUser(userType);
            CreateAllocation(user);

            var request = new AllocateUserRequestBuilder().WithUserType(userType).Build();

            QueryHandler
                .Setup(
                    x => x.Handle<GetAllocatedUserByUserTypeQuery, User>(It.IsAny<GetAllocatedUserByUserTypeQuery>()))
                .ReturnsAsync(user);

            var response = Controller.AllocateSingleUserAsync(request);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var userDetailsResponse = (UserDetailsResponse) result.Value;
            userDetailsResponse.Should().BeEquivalentTo(user);
        }

        [TestCase(TestType.Automated)]
        [TestCase(TestType.Demo)]
        [TestCase(TestType.ITHC)]
        [TestCase(TestType.Manual)]
        [TestCase(TestType.Performance)]
        public void Should_return_allocated_user_for_test_type(TestType testType)
        {
            var user = CreateUser(testType);
            CreateAllocation(user);

            var request = new AllocateUserRequestBuilder()
                .WithUserType(user.UserType)
                .ForTestType(testType)
                .Build();

            QueryHandler
                .Setup(
                    x => x.Handle<GetAllocatedUserByUserTypeQuery, User>(It.IsAny<GetAllocatedUserByUserTypeQuery>()))
                .ReturnsAsync(user);

            var response = Controller.AllocateSingleUserAsync(request);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var userDetailsResponse = (UserDetailsResponse) result.Value;
            userDetailsResponse.Should().BeEquivalentTo(user);
        }

        [Test]
        public void Should_allocate_prod_user()
        {
            const bool IS_PROD_USER = UserData.IS_PROD_USER;

            var user = CreateUser(UserType.Individual, IS_PROD_USER);
            CreateAllocation(user);

            var request = new AllocateUserRequestBuilder()
                .WithUserType(user.UserType)
                .IsProdUser()
                .Build();

            QueryHandler
                .Setup(
                    x => x.Handle<GetAllocatedUserByUserTypeQuery, User>(It.IsAny<GetAllocatedUserByUserTypeQuery>()))
                .ReturnsAsync(user);

            var response = Controller.AllocateSingleUserAsync(request);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var userDetailsResponse = (UserDetailsResponse) result.Value;
            userDetailsResponse.Should().BeEquivalentTo(user);
        }

        [TestCase(TestType.Automated)]
        [TestCase(TestType.Demo)]
        [TestCase(TestType.ITHC)]
        [TestCase(TestType.Manual)]
        [TestCase(TestType.Performance)]
        public void Should_create_user_first_names_based_on_test_type(TestType testType)
        {
            var user = CreateUser(testType);
            CreateAllocation(user);

            var request = new AllocateUserRequestBuilder()
                .WithUserType(user.UserType)
                .ForTestType(testType)
                .Build();

            QueryHandler
                .Setup(
                    x => x.Handle<GetAllocatedUserByUserTypeQuery, User>(It.IsAny<GetAllocatedUserByUserTypeQuery>()))
                .ReturnsAsync(user);

            var response = Controller.AllocateSingleUserAsync(request);
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var userDetailsResponse = (UserDetailsResponse)result.Value;
            userDetailsResponse.Should().BeEquivalentTo(user);

            userDetailsResponse.FirstName.ToLower().Should().Contain(testType == TestType.Automated
                ? UserData.AUTOMATED_FIRST_NAME_PREFIX.ToLower()
                : testType.ToString().ToLower());
        }
    }
}