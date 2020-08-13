﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TestApi.DAL.Commands;
using TestApi.DAL.Helpers;
using TestApi.DAL.Queries;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.UnitTests.Services
{
    public class NewUserAllocationServiceTests : ServicesTestBase
    {
        [Test]
        public async Task Should_allocate_new_user_no_users_exist_user_exists_in_aad()
        {
            var users = new List<User>();

            QueryHandler
                .Setup(x => x.Handle<GetAllUsersByUserTypeQuery, List<User>>(It.IsAny<GetAllUsersByUserTypeQuery>()))
                .ReturnsAsync(users);

            const int NUMBER = 1;

            QueryHandler
                .Setup(x =>
                    x.Handle<GetNextUserNumberByUserTypeQuery, Integer>(It.IsAny<GetNextUserNumberByUserTypeQuery>()))
                .ReturnsAsync(new Integer(NUMBER));

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<CreateNewUserCommand>()))
                .Returns(Task.CompletedTask);

            var user = CreateNewUser(UserType.Individual, NUMBER);

            QueryHandler
                .Setup(x => x.Handle<GetUserByUserTypeAppAndNumberQuery, User>(
                    It.IsAny<GetUserByUserTypeAppAndNumberQuery>()))
                .ReturnsAsync(user);

            var allocation = CreateAllocation(user);

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<CreateNewAllocationByUserIdCommand>()))
                .Returns(Task.FromResult(allocation));

            QueryHandler
                .Setup(x => x.Handle<GetUserByIdQuery, User>(It.IsAny<GetUserByIdQuery>()))
                .ReturnsAsync(user);

            MockUserApiService
                .Setup(x => x.CheckUserExistsInAAD(It.IsAny<string>()))
                .ReturnsAsync(true);

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<AllocateByUserIdCommand>()))
                .Returns(Task.CompletedTask);

            const int MINUTES = 1;

            var allocatedUser = await AllocationService.AllocateToService(user.UserType, user.Application, MINUTES);
            allocatedUser.Should().BeEquivalentTo(user);
        }

        [Test]
        public async Task Should_allocate_new_user_all_users_allocated_user_exists_in_aad()
        {
            const int NUMBER_OF_USERS = 3;
            const UserType USER_TYPE = UserType.Individual;
            var users = CreateListOfUsers(USER_TYPE, NUMBER_OF_USERS);
            var allocations = CreateAllocations(users);
            allocations.Count.Should().Be(NUMBER_OF_USERS);
            AllocateAllUsers(allocations);

            QueryHandler
                .Setup(x => x.Handle<GetAllUsersByUserTypeQuery, List<User>>(It.IsAny<GetAllUsersByUserTypeQuery>()))
                .ReturnsAsync(users);

            QueryHandler
                .SetupSequence(x =>
                    x.Handle<GetAllocationByUserIdQuery, Allocation>(It.IsAny<GetAllocationByUserIdQuery>()))
                .ReturnsAsync(allocations[0])
                .ReturnsAsync(allocations[1])
                .ReturnsAsync(allocations[2]);

            var number = users.Count + 1;

            QueryHandler
                .Setup(x =>
                    x.Handle<GetNextUserNumberByUserTypeQuery, Integer>(It.IsAny<GetNextUserNumberByUserTypeQuery>()))
                .ReturnsAsync(new Integer(number));

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<CreateNewUserCommand>()))
                .Returns(Task.CompletedTask);

            var user = CreateNewUser(USER_TYPE, number);

            QueryHandler
                .Setup(x => x.Handle<GetUserByUserTypeAppAndNumberQuery, User>(
                    It.IsAny<GetUserByUserTypeAppAndNumberQuery>()))
                .ReturnsAsync(user);

            var allocation = CreateAllocation(user);

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<CreateNewAllocationByUserIdCommand>()))
                .Returns(Task.FromResult(allocation));

            QueryHandler
                .Setup(x => x.Handle<GetUserByIdQuery, User>(It.IsAny<GetUserByIdQuery>()))
                .ReturnsAsync(user);

            MockUserApiService
                .Setup(x => x.CheckUserExistsInAAD(It.IsAny<string>()))
                .ReturnsAsync(true);

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<AllocateByUserIdCommand>()))
                .Returns(Task.CompletedTask);

            const int MINUTES = 1;

            var allocatedUser = await AllocationService.AllocateToService(user.UserType, user.Application, MINUTES);
            allocatedUser.Should().BeEquivalentTo(user);
        }

        [Test]
        public async Task Should_allocate_new_user_no_users_exist_does_not_exist_in_aad()
        {
            var users = new List<User>();

            QueryHandler
                .Setup(x => x.Handle<GetAllUsersByUserTypeQuery, List<User>>(It.IsAny<GetAllUsersByUserTypeQuery>()))
                .ReturnsAsync(users);

            const int NUMBER = 1;

            QueryHandler
                .Setup(x =>
                    x.Handle<GetNextUserNumberByUserTypeQuery, Integer>(It.IsAny<GetNextUserNumberByUserTypeQuery>()))
                .ReturnsAsync(new Integer(NUMBER));

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<CreateNewUserCommand>()))
                .Returns(Task.CompletedTask);

            var user = CreateNewUser(UserType.Individual, NUMBER);

            QueryHandler
                .Setup(x => x.Handle<GetUserByUserTypeAppAndNumberQuery, User>(
                    It.IsAny<GetUserByUserTypeAppAndNumberQuery>()))
                .ReturnsAsync(user);

            var allocation = CreateAllocation(user);

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<CreateNewAllocationByUserIdCommand>()))
                .Returns(Task.FromResult(allocation));

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
                .Setup(x => x.CreateNewUserInAAD(It.IsAny<ADUser>()))
                .ReturnsAsync(newUserResponse);

            CommandHandler
                .Setup(x => x.Handle(It.IsAny<AllocateByUserIdCommand>()))
                .Returns(Task.CompletedTask);

            const int MINUTES = 1;

            var allocatedUser = await AllocationService.AllocateToService(user.UserType, user.Application, MINUTES);
            allocatedUser.Should().BeEquivalentTo(user);
        }
    }
}