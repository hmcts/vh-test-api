using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Responses;

namespace TestApi.UnitTests.Controllers.Conferences
{
    public class GetAudioRecordingLinkControllerTests : ConferencesControllerTestsBase
    {
        [Test]
        public async Task Should_get_audio_recording_link()
        {
            var audioLinkResponse = new AudioRecordingResponse()
            {
                AudioFileLinks = new List<string>(){ "http://link-to-file" }
            };
                
            VideoApiClient
                .Setup(x => x.GetAudioRecordingLinkAsync(It.IsAny<Guid>()))
                .ReturnsAsync(audioLinkResponse);

            var response = await Controller.GetAudioRecordingLinksByHearingId(Guid.NewGuid());
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var recordingDetails = (AudioRecordingResponse)result.Value;
            recordingDetails.Should().NotBeNull();
            recordingDetails.Should().BeEquivalentTo(audioLinkResponse);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing_id()
        {
            VideoApiClient
                .Setup(x => x.GetAudioRecordingLinkAsync(It.IsAny<Guid>()))
                .ThrowsAsync(CreateVideoApiException("No hearing found", HttpStatusCode.NotFound));

            var response = await Controller.GetAudioRecordingLinksByHearingId(Guid.NewGuid());
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
