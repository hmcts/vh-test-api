using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.UnitTests.Controllers.Conferences
{
    public class CreateVideoEventControllerTests : ConferencesControllerTestsBase
    {
        [Test]
        public async Task Should_create_video_event()
        {
            const EventType EVENT_TYPE = EventType.None;
            const int EVENT_TYPE_ID = (int)EVENT_TYPE;

            var request = new ConferenceEventRequest()
            {
                Conference_id = Guid.NewGuid().ToString(),
                Event_id = EVENT_TYPE_ID.ToString(),
                Event_type = EVENT_TYPE,
                Participant_id = Guid.NewGuid().ToString(),
                Phone = string.Empty,
                Reason = HearingData.VIDEO_EVENT_REASON,
                Time_stamp_utc = DateTime.UtcNow,
                Transfer_from = null,
                Transfer_to = null
            };

            VideoApiClient
                .Setup(x => x.RaiseVideoEventAsync(It.IsAny<ConferenceEventRequest>()))
                .Returns(Task.CompletedTask);

            var response = await Controller.CreateVideoEventAsync(request);
            response.Should().NotBeNull();

            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }
}
