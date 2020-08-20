using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.Controllers;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries.Core;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.UnitTests.Controllers.Conferences
{
    public class ConferencesControllerTestsBase : ControllerTestsBase
    {
        protected Mock<ICommandHandler> CommandHandler;
        protected ConferencesController Controller;
        protected Mock<ILogger<ConferencesController>> Logger;
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<IVideoApiClient> VideoApiClient;

        [SetUp]
        public void Setup()
        {
            QueryHandler = new Mock<IQueryHandler>();
            CommandHandler = new Mock<ICommandHandler>();
            Logger = new Mock<ILogger<ConferencesController>>();
            VideoApiClient = new Mock<IVideoApiClient>();
            Controller = new ConferencesController(Logger.Object, VideoApiClient.Object);
        }
    }
}
