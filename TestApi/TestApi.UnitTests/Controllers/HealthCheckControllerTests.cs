using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Contract.Responses;
using TestApi.Controllers;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Contracts;

namespace TestApi.UnitTests.Controllers
{
    public class HealthCheckControllerTests
    {
        private HealthCheckController _controller;
        private Mock<IQueryHandler> _mockQueryHandler;
        private Mock<IBookingsApiService> _mockBookingsApiService;
        private Mock<IUserApiService> _mockUserApiService;
        private Mock<IVideoApiService> _mockVideoApiService;

        [SetUp]
        public void Setup()
        {
            _mockQueryHandler = new Mock<IQueryHandler>();
            _mockBookingsApiService = new Mock<IBookingsApiService>();
            _mockUserApiService = new Mock<IUserApiService>();
            _mockVideoApiService = new Mock<IVideoApiService>();
        }

        [Test]
        public async Task Should_return_ok_result_when_database_is_connected()
        {
            const string emailStem = "made_up_email_stem_for_test";
            const int userNumber = 1;
            const Application application = Application.TestApi;

            var user = new UserBuilder(emailStem, userNumber)
                .WithUserType(UserType.Judge)
                .ForApplication(application)
                .BuildUser();

            var query = new GetUserByUsernameQuery(user.Username);
            
            _controller = new HealthCheckController(_mockQueryHandler.Object, _mockBookingsApiService.Object, _mockUserApiService.Object, _mockVideoApiService.Object);
            _mockQueryHandler.Setup(x => x.Handle<GetUserByUsernameQuery, User>(query))
                .Returns(Task.FromResult(user));

            var result = await _controller.HealthAsync();
            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_internal_server_error_result_when_database_is_not_connected()
        {
            var exception = new AggregateException("database connection failed");

            _controller = new HealthCheckController(_mockQueryHandler.Object, _mockBookingsApiService.Object, _mockUserApiService.Object, _mockVideoApiService.Object);
            _mockQueryHandler
                .Setup(x => x.Handle<GetUserByUsernameQuery, User>(It.IsAny<GetUserByUsernameQuery>()))
                .ThrowsAsync(exception);

            var result = await _controller.HealthAsync();
            var typedResult = (ObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            var response = (HealthCheckResponse)typedResult.Value;
            response.TestApiHealth.Successful.Should().BeFalse();
            response.TestApiHealth.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task Should_return_the_application_version_from_assembly()
        {
            var result = await _controller.HealthAsync();
            var typedResult = (ObjectResult)result;
            var response = (HealthCheckResponse)typedResult.Value;
            response.Version.Version.Should().NotBeNullOrEmpty();
        }
    }
}