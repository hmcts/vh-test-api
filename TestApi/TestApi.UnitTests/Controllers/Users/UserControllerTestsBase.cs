using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.Controllers;
using TestApi.DAL.Queries.Core;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Contracts;

namespace TestApi.UnitTests.Controllers.Users
{
    public class UserControllerTestsBase
    {
        protected UserController Controller;
        protected Mock<ILogger<UserController>> Logger;
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<IUserApiClient> UserApiClient;
        protected Mock<IUserApiService> UserApiService;

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