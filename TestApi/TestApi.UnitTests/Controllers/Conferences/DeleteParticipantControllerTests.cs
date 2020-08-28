using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace TestApi.UnitTests.Controllers.Conferences
{
    public class DeleteParticipantControllerTests : ConferencesControllerTestsBase
    {
        [Test]
        public async Task Should_delete_participant()
        {
            VideoApiClient
                .Setup(x => x.RemoveParticipantFromConferenceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            var response = await Controller.DeleteParticipantAsync(Guid.NewGuid(), Guid.NewGuid());
            response.Should().NotBeNull();

            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_conference_or_participant_ids()
        {
            VideoApiClient
                .Setup(x => x.RemoveParticipantFromConferenceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(CreateVideoApiException("Conference not found", HttpStatusCode.NotFound));

            var response = await Controller.DeleteParticipantAsync(Guid.NewGuid(), Guid.NewGuid());
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
