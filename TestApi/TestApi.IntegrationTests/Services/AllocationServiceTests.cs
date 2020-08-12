using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.DAL.Commands;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Services.Contracts;

namespace TestApi.IntegrationTests.Services
{
    public class AllocationServiceTests : ServicesTestBase
    {
        private readonly IAllocationService _service;
        private readonly Mock<ICommandHandler> _commandHandler;
        private readonly Mock<IQueryHandler> _queryHandler;
        private readonly Mock<ILogger<AllocationService>> _mockLogger;
        private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _configuration;

        private readonly Mock<IUserApiService> _userApiService;

        public AllocationServiceTests()
        {
            _commandHandler = new Mock<ICommandHandler>();
            _queryHandler = new Mock<IQueryHandler>();
            _mockLogger = new Mock<ILogger<AllocationService>>();
            _configuration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            _userApiService = new Mock<IUserApiService>();
            _service = new AllocationService(_commandHandler.Object, _queryHandler.Object, _mockLogger.Object, _configuration.Object, _userApiService.Object);
        }

        [Test]
        public async Task Should_allocate_new_user()
        {
            //const UserType USERTYPE = UserType.Judge;
            //const Application APPLICATION = Application.TestApi;
            //const int MINUTES = 1;

            //_queryHandler
            //  .Setup(x => x.Handle<GetAllUsersByUserTypeQuery, List<User>>(It.IsAny<GetAllUsersByUserTypeQuery>()))
            //  .ReturnsAsync(new List<User>());

            //_commandHandler
            //    .Setup(x => x.Handle(It.IsAny<CreateNewAllocationByUserIdCommand>()))
            //    .Returns(Task.FromResult(default(object)));

            //_queryHandler
            //    .Setup(x => x.Handle<GetAllocationByUserIdQuery, Allocation>(It.IsAny<GetAllUsersByUserTypeQuery>()))
            //    .ReturnsAsync(new List<User>());
            //GetAllocationByUserId

            //            var userDetails = await _service.AllocateToService(USERTYPE, APPLICATION, MINUTES);
        }
        
    }
}
