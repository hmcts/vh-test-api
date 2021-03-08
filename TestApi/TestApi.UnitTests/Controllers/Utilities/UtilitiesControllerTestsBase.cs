using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.Controllers;
using TestApi.Services.Services;
using VideoApi.Client;

namespace TestApi.UnitTests.Controllers.Utilities
{
    public class UtilitiesControllerTestsBase : ControllerTestsBase
    {
        protected UtilitiesController Controller;
        protected Mock<ILogger<UtilitiesController>> Logger;
        protected Mock<IBookingsApiService> BookingsApiService;
        protected Mock<IVideoApiClient> VideoApiClient;

        [SetUp]
        public void Setup()
        {
            Logger = new Mock<ILogger<UtilitiesController>>();
            BookingsApiService = new Mock<IBookingsApiService>();
            VideoApiClient = new Mock<IVideoApiClient>();
            Controller = new UtilitiesController(Logger.Object, BookingsApiService.Object, VideoApiClient.Object);
        }
    }
}
