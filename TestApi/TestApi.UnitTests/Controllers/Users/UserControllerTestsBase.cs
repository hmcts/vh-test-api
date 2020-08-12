using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.Controllers;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries.Core;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Contracts;

namespace TestApi.UnitTests.Controllers.Users
{
    public class UserControllerTestsBase
    {
        protected UserController Controller;
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<ILogger<UserController>> Logger;
        protected Mock<IUserApiService> UserApiService;
        protected Mock<IUserApiClient> UserApiClient;

        [SetUp]
        public void OneTimeSetUp()
        {
            QueryHandler = new Mock<IQueryHandler>();
            Logger = new Mock<ILogger<UserController>>();
            UserApiClient = new Mock<IUserApiClient>();
            UserApiService = new Mock<IUserApiService>();
            Controller = new UserController(QueryHandler.Object, Logger.Object, UserApiService.Object);
        }
    }
}
