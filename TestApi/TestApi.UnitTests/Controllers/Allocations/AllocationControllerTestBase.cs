using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using TestApi.Common.Configuration;
using TestApi.Contract.Builders;
using TestApi.Controllers;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.UnitTests.Controllers.Allocations
{
    public class AllocationControllerTestBase
    {
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected AllocationController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ILogger<AllocationController>> MockLogger;
        protected Mock<IOptions<ServicesConfiguration>> ServicesConfiguration;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            MockLogger = new Mock<ILogger<AllocationController>>();
            ServicesConfiguration = new Mock<IOptions<ServicesConfiguration>>();

            //QueryHandlerMock
            //    .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
            //    .ReturnsAsync(TestConference);

            //QueryHandlerMock
            //    .Setup(x => x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByHearingRefIdQuery>()))
            //    .ReturnsAsync(TestConference);
            //QueryHandlerMock
            //  .Setup(x => x.Handle<GetExpiredAudiorecordingConferencesQuery, List<VideoApi.Domain.Conference>>(It.IsAny<GetExpiredAudiorecordingConferencesQuery>()))
            //  .ReturnsAsync(new List<VideoApi.Domain.Conference> { TestConference });

            //CommandHandlerMock
            //    .Setup(x => x.Handle(It.IsAny<SaveEventCommand>()))
            //    .Returns(Task.FromResult(default(object)));

            ServicesConfiguration.Setup(s => s.Value).Returns(new ServicesConfiguration());

            Controller = new AllocationController( CommandHandlerMock.Object, QueryHandlerMock.Object, MockLogger.Object);
        }

        protected static User CreateUser()
        {
            const string emailStem = "made_up_email_stem";
            const int number = 1;
            return new UserBuilder(emailStem, number)
                .WithUserType(UserType.CaseAdmin)
                .ForApplication(Application.TestApi)
                .BuildUser();
        }

        protected static Allocation CreateAllocation(User user)
        {
            return new Allocation(user);
        }
    }
}
