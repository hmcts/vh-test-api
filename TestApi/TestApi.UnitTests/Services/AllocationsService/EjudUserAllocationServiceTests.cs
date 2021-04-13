using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Dtos;
using TestApi.DAL.Commands;
using TestApi.DAL.Queries;
using TestApi.Domain;
using TestApi.Contract.Enums;
using TestApi.DAL.Exceptions;

namespace TestApi.UnitTests.Services.AllocationsService
{
    public class EjudUserAllocationServiceTests : ServicesTestBase
    {
        [Test]
        public async Task Should_allocate_ejud_joh_user()
        {
            var users = new List<UserDto>();

            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var judge = new EjudUserBuilder(EMAIL_STEM, 1).AddJudge().BuildUserDto();
            users.Add(judge);

            QueryHandler
                .Setup(x => x.Handle<GetAllUsersByFilterQuery, List<UserDto>>(It.IsAny<GetAllUsersByFilterQuery>()))
                .ReturnsAsync(users);

            var allocation = CreateAllocation(judge);

            QueryHandler
                .Setup(x => x.Handle<GetAllocationByUserIdQuery, Allocation>(It.IsAny<GetAllocationByUserIdQuery>()))
                .ReturnsAsync(allocation);

            QueryHandler
                    .Setup(x => x.Handle<GetUserByIdQuery, UserDto>(It.IsAny<GetUserByIdQuery>()))
                    .ReturnsAsync(judge);

            CommandHandler
                    .Setup(x => x.Handle(It.IsAny<AllocateByUserIdCommand>()))
                    .Returns(Task.CompletedTask);

            var allocatedUser = await AllocationService.AllocateJudicialOfficerHolderToService(judge.TestType);
           allocatedUser.Should().BeEquivalentTo(judge);
        }

        [Test]
        public void Should_throw_error_if_no_ejud_users_exist()
        {
            QueryHandler
                .Setup(x => x.Handle<GetAllUsersByFilterQuery, List<UserDto>>(It.IsAny<GetAllUsersByFilterQuery>()))
                .ReturnsAsync(new List<UserDto>());

            Assert.ThrowsAsync<NoEjudUsersExistException>(async () => await AllocationService.AllocateJudicialOfficerHolderToService(TestType.Automated));
        }

        [Test]
        public void Should_throw_error_when_all_users_allocated()
        {
            var users = new List<UserDto>();

            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;

            var judge = new EjudUserBuilder(EMAIL_STEM, 1).AddJudge().BuildUserDto();
            users.Add(judge);

            QueryHandler
                .Setup(x => x.Handle<GetAllUsersByFilterQuery, List<UserDto>>(It.IsAny<GetAllUsersByFilterQuery>()))
                .ReturnsAsync(users);

            var allocation = CreateAllocation(judge);
            const int ALLOCATE_FOR_MINUTES = 1;
            allocation.Allocate(ALLOCATE_FOR_MINUTES, EmailData.TEST_WEB_MANUAL_USER);

            QueryHandler
                .Setup(x => x.Handle<GetAllocationByUserIdQuery, Allocation>(It.IsAny<GetAllocationByUserIdQuery>()))
                .ReturnsAsync(allocation);

            Assert.ThrowsAsync<AllUsersAreAllocatedException>(async () =>
                await AllocationService.AllocateJudicialOfficerHolderToService(TestType.Automated));
        }
    }
}