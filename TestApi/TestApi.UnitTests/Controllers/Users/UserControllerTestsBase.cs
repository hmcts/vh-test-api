using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Controllers;
using TestApi.DAL.Queries.Core;
using TestApi.Services.Services;
using UserApi.Client;

namespace TestApi.UnitTests.Controllers.Users
{
    public class UserControllerTestsBase
    {
        protected UserController Controller;
        protected Mock<ILogger<UserController>> Logger;
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<IUserApiClient> UserApiClient;
        protected Mock<IUserApiService> UserApiService;
        protected Mock<IConfiguration> Configuration;

        [SetUp]
        public void OneTimeSetUp()
        {
            QueryHandler = new Mock<IQueryHandler>();
            Logger = new Mock<ILogger<UserController>>();
            UserApiClient = new Mock<IUserApiClient>();
            UserApiService = new Mock<IUserApiService>();
            Configuration = new Mock<IConfiguration>();
            SetMockConfig();
            Controller = new UserController(QueryHandler.Object, Logger.Object, UserApiService.Object, UserApiClient.Object, Configuration.Object);
        }

        private void SetMockConfig()
        {
            Configuration
                .Setup(x => x.GetSection("EjudUsernameStem").Value)
                .Returns(EjudUserData.FAKE_EJUD_DOMAIN);

            Configuration
                .Setup(x => x.GetSection("TestDefaultPassword").Value)
                .Returns(EjudUserData.FAKE_PASSWORD);
        }
    }
}