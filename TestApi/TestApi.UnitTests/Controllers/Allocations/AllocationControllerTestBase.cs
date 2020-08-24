using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Controllers;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.UnitTests.Controllers.Allocations
{
    public class HearingsControllerTestBase
    {
        protected Mock<ICommandHandler> CommandHandler;
        protected AllocationController Controller;
        protected Mock<ILogger<AllocationController>> Logger;
        protected Mock<IQueryHandler> QueryHandler;

        [SetUp]
        public void Setup()
        {
            QueryHandler = new Mock<IQueryHandler>();
            CommandHandler = new Mock<ICommandHandler>();
            Logger = new Mock<ILogger<AllocationController>>();
            Controller = new AllocationController(CommandHandler.Object, QueryHandler.Object, Logger.Object);
        }

        protected static User CreateUser(UserType userType, bool isProdUser = false)
        {
            const string emailStem = EmailData.FAKE_EMAIL_STEM;
            const int number = 1;
            return new UserBuilder(emailStem, number)
                .WithUserType(userType)
                .ForApplication(Application.TestApi)
                .IsProdUser(isProdUser)
                .BuildUser();
        }

        protected static User CreateUser(TestType testType)
        {
            const string emailStem = EmailData.FAKE_EMAIL_STEM;
            const int number = 1;
            return new UserBuilder(emailStem, number)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUser();
        }

        protected static Allocation CreateAllocation(User user)
        {
            return new Allocation(user);
        }

        protected static Allocation Unallocate(Allocation allocation)
        {
            allocation.Unallocate();
            return allocation;
        }
    }
}