using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.DAL.Commands;
using TestApi.DAL.Queries;
using TestApi.Contract.Enums;

namespace TestApi.IntegrationTests.Database.Queries
{
    public class GetAllocatedUserByUserTypeQueryTests : DatabaseTestsBase
    {
        private readonly GetAllocatedUserByUserTypeQueryHandler _query;
        private readonly Mock<IAllocationService> _allocationService;

        public GetAllocatedUserByUserTypeQueryTests()
        {
            _allocationService = new Mock<IAllocationService>();
            _query = new GetAllocatedUserByUserTypeQueryHandler(DbContext, _allocationService.Object);
        }

        [Test]
        public async Task Should_allocate_user_no_users_exist()
        {
            const UserType USER_TYPE = UserType.Individual;
            const Application APPLICATION = Application.TestApi;
            const string USERNAME_STEM = EmailData.FAKE_EMAIL_STEM;
            const int NUMBER = 1;

            var user = new UserBuilder(USERNAME_STEM, NUMBER)
                .WithUserType(USER_TYPE)
                .ForApplication(APPLICATION)
                .BuildUserDto();

            var allocationRequest = new AllocateUserRequestBuilder().WithUserType(USER_TYPE).Build();

            _allocationService
                .Setup(x => x.AllocateToService(It.IsAny<UserType>(), It.IsAny<Application>(), It.IsAny<TestType>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult(user));

            var userDetails = await _query.Handle(new GetAllocatedUserByUserTypeQuery(allocationRequest));
            userDetails.Should().NotBeNull();
            userDetails.Should().BeEquivalentTo(user);
        }
    }
}
