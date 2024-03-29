using System;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Client;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Dtos;
using TestApi.Contract.Responses;
using TestApi.Controllers;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Contract.Enums;
using UserApi.Client;
using VideoApi.Client;

namespace TestApi.UnitTests.Controllers
{
    public class HealthControllerTests
    {
        private HealthController _controller;
        private Mock<IBookingsApiClient> _mockBookingsApiClient;
        private Mock<IQueryHandler> _mockQueryHandler;
        private Mock<IUserApiClient> _mockUserApiClient;
        private Mock<IVideoApiClient> _mockVideoApiClient;
        private Mock<ILogger<HealthController>> _logger;

        [SetUp]
        public void Setup()
        {
            _mockQueryHandler = new Mock<IQueryHandler>();
            _mockBookingsApiClient = new Mock<IBookingsApiClient>();
            _mockUserApiClient = new Mock<IUserApiClient>();
            _mockVideoApiClient = new Mock<IVideoApiClient>();
            _logger = new Mock<ILogger<HealthController>>();
        }

        [Test]
        public async Task Should_return_ok_result_when_database_is_connected()
        {
            const string emailStem = EmailData.FAKE_EMAIL_STEM;
            const int userNumber = 1;
            const Application application = Application.TestApi;

            var user = new UserBuilder(emailStem, userNumber)
                .WithUserType(UserType.Judge)
                .ForApplication(application)
                .BuildUserDto();

            var query = new GetUserByUsernameQuery(user.Username);

            _controller = new HealthController(_mockQueryHandler.Object, _mockBookingsApiClient.Object,
                _mockUserApiClient.Object, _mockVideoApiClient.Object, _logger.Object);
            _mockQueryHandler.Setup(x => x.Handle<GetUserByUsernameQuery, UserDto>(query))
                .Returns(Task.FromResult(user));

            var result = await _controller.Health();
            var typedResult = (OkObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_internal_server_error_result_when_database_is_not_connected()
        {
            var exception = new AggregateException("database connection failed");

            _controller = new HealthController(_mockQueryHandler.Object, _mockBookingsApiClient.Object,
                _mockUserApiClient.Object, _mockVideoApiClient.Object, _logger.Object);
            _mockQueryHandler
                .Setup(x => x.Handle<GetUserByUsernameQuery, UserDto>(It.IsAny<GetUserByUsernameQuery>()))
                .ThrowsAsync(exception);

            var result = await _controller.Health();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            var response = (HealthResponse) typedResult.Value;
            response.TestApiHealth.Successful.Should().BeFalse();
            response.TestApiHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task Should_return_the_application_version_from_assembly()
        {
            var result = await _controller.Health();
            var typedResult = (ObjectResult) result;
            var response = (HealthResponse) typedResult.Value;
            response.Version.FileVersion.Should().NotBeNullOrEmpty();
            response.Version.InformationVersion.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task Should_return_the_bookings_api_health()
        {
            var result = await _controller.Health();
            var typedResult = (ObjectResult) result;
            var response = (HealthResponse) typedResult.Value;
            response.BookingsApiHealth.Successful.Should().BeTrue();
            response.BookingsApiHealth.ErrorMessage.Should().BeNullOrWhiteSpace();
            response.VideoApiHealth.Data.Should().BeNull();
        }

        [Test]
        public async Task Should_return_the_user_api_health()
        {
            var result = await _controller.Health();
            var typedResult = (ObjectResult) result;
            var response = (HealthResponse) typedResult.Value;
            response.UserApiHealth.Successful.Should().BeTrue();
            response.UserApiHealth.ErrorMessage.Should().BeNullOrWhiteSpace();
            response.VideoApiHealth.Data.Should().BeNull();
        }

        [Test]
        public async Task Should_return_the_video_api_health()
        {
            var result = await _controller.Health();
            var typedResult = (ObjectResult) result;
            var response = (HealthResponse) typedResult.Value;
            response.VideoApiHealth.Successful.Should().BeTrue();
            response.VideoApiHealth.ErrorMessage.Should().BeNullOrWhiteSpace();
            response.VideoApiHealth.Data.Should().BeNull();
        }

        [Test]
        public async Task Should_return_internal_server_error_result_when_bookings_api_not_reachable()
        {
            var exception = new AggregateException("BQS error");

            _controller = new HealthController(_mockQueryHandler.Object, _mockBookingsApiClient.Object,
                _mockUserApiClient.Object, _mockVideoApiClient.Object, _logger.Object);

            _mockBookingsApiClient
                .Setup(x => x.CheckServiceHealthAsync())
                .ThrowsAsync(exception);

            var result = await _controller.Health();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            var response = (HealthResponse) typedResult.Value;
            response.BookingsApiHealth.Successful.Should().BeFalse();
            response.BookingsApiHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task Should_return_internal_server_error_result_when_user_api_not_reachable()
        {
            var exception = new AggregateException("AAD error");

            _controller = new HealthController(_mockQueryHandler.Object, _mockBookingsApiClient.Object,
                _mockUserApiClient.Object, _mockVideoApiClient.Object, _logger.Object);

            _mockUserApiClient
                .Setup(x => x.CheckServiceHealthAsync())
                .ThrowsAsync(exception);

            var result = await _controller.Health();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            var response = (HealthResponse) typedResult.Value;
            response.UserApiHealth.Successful.Should().BeFalse();
            response.UserApiHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task Should_return_internal_server_error_result_when_video_api_not_reachable()
        {
            var exception = new AggregateException("kinly api error");

            _controller = new HealthController(_mockQueryHandler.Object, _mockBookingsApiClient.Object,
                _mockUserApiClient.Object, _mockVideoApiClient.Object, _logger.Object);

            _mockVideoApiClient
                .Setup(x => x.CheckServiceHealthAsync())
                .ThrowsAsync(exception);

            var result = await _controller.Health();
            var typedResult = (ObjectResult) result;
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
            var response = (HealthResponse) typedResult.Value;
            response.VideoApiHealth.Successful.Should().BeFalse();
            response.VideoApiHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }
    }
}