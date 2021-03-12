using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Services.Builders.Requests;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;

namespace TestApi.UnitTests.Controllers.Conferences
{
    public class CreateEventControllerTests : ConferencesControllerTestsBase
    {
        [Test]
        public async Task Should_create_event()
        {
            var request = new ConferenceEventRequestBuilder()
                .WithEventType(EventType.None)
                .Build();

            VideoApiClient
                .Setup(x => x.RaiseVideoEventAsync(It.IsAny<ConferenceEventRequest>()))
                .Returns(Task.CompletedTask);

            var response = await Controller.CreateEvent(request);
            response.Should().NotBeNull();

            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }
}
