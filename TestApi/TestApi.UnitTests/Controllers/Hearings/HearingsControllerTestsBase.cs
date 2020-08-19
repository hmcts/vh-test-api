using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.Controllers;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries.Core;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Contracts;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class HearingsControllerTestsBase : ControllerTestsBase
    {
        protected Mock<ICommandHandler> CommandHandler;
        protected HearingsController Controller;
        protected Mock<ILogger<HearingsController>> Logger;
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<IBookingsApiClient> BookingsApiClient;
        protected Mock<IBookingsApiService> BookingsApiService;
        protected Mock<IVideoApiService> VideoApiService;

        [SetUp]
        public void Setup()
        {
            QueryHandler = new Mock<IQueryHandler>();
            CommandHandler = new Mock<ICommandHandler>();
            Logger = new Mock<ILogger<HearingsController>>();
            BookingsApiClient = new Mock<IBookingsApiClient>();
            BookingsApiService = new Mock<IBookingsApiService>();
            VideoApiService = new Mock<IVideoApiService>();
            Controller = new HearingsController(Logger.Object, BookingsApiClient.Object, BookingsApiService.Object, VideoApiService.Object);
        }
    }
}