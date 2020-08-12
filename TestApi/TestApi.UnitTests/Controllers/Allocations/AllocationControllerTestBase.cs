using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Controllers;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.UnitTests.Controllers.Allocations
{
    public class HearingsControllerTestBase
    {
        protected AllocationController Controller;
        protected Mock<ICommandHandler> CommandHandler;
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<ILogger<AllocationController>> Logger;

        [SetUp]
        public void Setup()
        {
            QueryHandler = new Mock<IQueryHandler>();
            CommandHandler = new Mock<ICommandHandler>();
            Logger = new Mock<ILogger<AllocationController>>();
            Controller = new AllocationController(CommandHandler.Object, QueryHandler.Object, Logger.Object);
        }

        protected static User CreateUser(UserType userType)
        {
            const string emailStem = "made_up_email_stem";
            const int number = 1;
            return new UserBuilder(emailStem, number)
                .WithUserType(userType)
                .ForApplication(Application.TestApi)
                .BuildUser();
        }

        protected static Allocation CreateAllocation(User user)
        {
            return new Allocation(user);
        }
    }
}
