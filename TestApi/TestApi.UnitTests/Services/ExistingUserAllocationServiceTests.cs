using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.DAL.Commands;
using TestApi.DAL.Queries;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.UnitTests.Services
{
    public class ExistingUserAllocationServiceTests : ServicesTestBase
    {
        [Test]
        public async Task Should_allocate_existing_user_with_allocations_exists_in_aad()
        {
            const int NUMBER_OF_USERS = 3;
            var users = CreateListOfUsers(UserType.Individual, NUMBER_OF_USERS);
            var allocations = CreateAllocations(users);
            allocations.Count.Should().Be(NUMBER_OF_USERS);

            QueryHandler
                .Setup(x => x.Handle<GetAllUsersByFilterQuery, List<User>>(It.IsAny<GetAllUsersByFilterQuery>()))
                .ReturnsAsync(users);

            QueryHandler
                .SetupSequence(x =>
                    x.Handle<GetAllocationByUserIdQuery, Allocation>(It.IsAny<GetAllocationByUserIdQuery>()))
                .ReturnsAsync(allocations[0])
                .ReturnsAsync(allocations[1])
                .ReturnsAsync(allocations[2]);

            var user = users.First();

            QueryHandler
                .Setup(x => x.Handle<GetUserByIdQuery, User>(It.IsAny<GetUserByIdQuery>()))
                .ReturnsAsync(user);

            MockUserApiService
                .Setup(x => x.CheckUserExistsInAAD(It.IsAny<string>()))
                .ReturnsAsync(true);

            var recentUser = new RecentUser(user.Username);

            QueryHandler
                .Setup(
                    x => x.Handle<GetRecentUserByUsernameQuery, RecentUser>(It.IsAny<GetRecentUserByUsernameQuery>()))
                .ReturnsAsync(recentUser);

            const int NUMBER_OF_USER_GROUPS = 2;

            MockUserApiService
                .Setup(x => x.AddGroupsToUser(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(NUMBER_OF_USER_GROUPS);

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<AllocateByUserIdCommand>()))
                .Returns(Task.CompletedTask);

            const int MINUTES = 1;

            var allocatedUser = await AllocationService.AllocateToService(user.UserType, user.Application, user.TestType, user.IsProdUser, MINUTES);
            allocatedUser.Should().BeEquivalentTo(user);
        }

        [Test]
        public async Task Should_allocate_existing_user_with_no_allocations_exists_in_aad()
        {
            const int NUMBER_OF_USERS = 3;
            var users = CreateListOfUsers(UserType.Individual, NUMBER_OF_USERS);

            QueryHandler
                .Setup(x => x.Handle<GetAllUsersByFilterQuery, List<User>>(It.IsAny<GetAllUsersByFilterQuery>()))
                .ReturnsAsync(users);

            var allocations = CreateAllocations(users);

            QueryHandler
                .SetupSequence(x =>
                    x.Handle<GetAllocationByUserIdQuery, Allocation>(It.IsAny<GetAllocationByUserIdQuery>()))
                .ReturnsAsync((Allocation) null)
                .ReturnsAsync(allocations[0])
                .ReturnsAsync((Allocation) null)
                .ReturnsAsync(allocations[1])
                .ReturnsAsync((Allocation) null)
                .ReturnsAsync(allocations[2]);

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<CreateNewAllocationByUserIdCommand>()))
                .Returns(Task.CompletedTask);

            var user = users.First();

            QueryHandler
                .Setup(x => x.Handle<GetUserByIdQuery, User>(It.IsAny<GetUserByIdQuery>()))
                .ReturnsAsync(user);

            MockUserApiService
                .Setup(x => x.CheckUserExistsInAAD(It.IsAny<string>()))
                .ReturnsAsync(true);

            var recentUser = new RecentUser(user.Username);

            QueryHandler
                .Setup(
                    x => x.Handle<GetRecentUserByUsernameQuery, RecentUser>(It.IsAny<GetRecentUserByUsernameQuery>()))
                .ReturnsAsync(recentUser);

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<AllocateByUserIdCommand>()))
                .Returns(Task.CompletedTask);

            const int MINUTES = 1;

            var allocatedUser = await AllocationService.AllocateToService(user.UserType, user.Application, user.TestType, user.IsProdUser, MINUTES);
            allocatedUser.Should().BeEquivalentTo(user);
        }

        [Test]
        public async Task Should_allocate_existing_user_with_allocations_does_not_exist_in_aad()
        {
            const int NUMBER_OF_USERS = 3;
            var users = CreateListOfUsers(UserType.Individual, NUMBER_OF_USERS);
            var allocations = CreateAllocations(users);
            allocations.Count.Should().Be(NUMBER_OF_USERS);

            QueryHandler
                .Setup(x => x.Handle<GetAllUsersByFilterQuery, List<User>>(It.IsAny<GetAllUsersByFilterQuery>()))
                .ReturnsAsync(users);

            QueryHandler
                .SetupSequence(x =>
                    x.Handle<GetAllocationByUserIdQuery, Allocation>(It.IsAny<GetAllocationByUserIdQuery>()))
                .ReturnsAsync(allocations[0])
                .ReturnsAsync(allocations[1])
                .ReturnsAsync(allocations[2]);

            var user = users.First();

            QueryHandler
                .Setup(x => x.Handle<GetUserByIdQuery, User>(It.IsAny<GetUserByIdQuery>()))
                .ReturnsAsync(user);

            MockUserApiService
                .Setup(x => x.CheckUserExistsInAAD(It.IsAny<string>()))
                .ReturnsAsync(false);

            var newUserResponse = new NewUserResponse
            {
                One_time_password = "password",
                User_id = "1234",
                Username = user.Username
            };

            MockUserApiService
                .Setup(x => x.CreateNewUserInAAD(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(newUserResponse);

            const int NUMBER_OF_USER_GROUPS = 2;

            MockUserApiService
                .Setup(x => x.AddGroupsToUser(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(NUMBER_OF_USER_GROUPS);

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<AllocateByUserIdCommand>()))
                .Returns(Task.CompletedTask);

            const int MINUTES = 1;

            var allocatedUser = await AllocationService.AllocateToService(user.UserType, user.Application, user.TestType, user.IsProdUser, MINUTES);
            allocatedUser.Should().BeEquivalentTo(user);
        }
    }
}