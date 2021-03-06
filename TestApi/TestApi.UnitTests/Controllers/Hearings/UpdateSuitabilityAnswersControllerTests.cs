using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class UpdateSuitabilityAnswersControllerTests : HearingsControllerTestsBase
    {
        [Test]
        public async Task Should_update_suitability_answers()
        {
            BookingsApiClient
                .Setup(x => x.UpdateSuitabilityAnswersAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<SuitabilityAnswersRequest>>()))
                .Returns(Task.CompletedTask);

            var response = await Controller.UpdateSuitabilityAnswersAsync(Guid.NewGuid(), Guid.NewGuid(), new List<SuitabilityAnswersRequest>());
            response.Should().NotBeNull();

            var result = (NoContentResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing_id_or_participant_id()
        {
            BookingsApiClient
                .Setup(x => x.UpdateSuitabilityAnswersAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<SuitabilityAnswersRequest>>()))
                .ThrowsAsync(CreateBookingsApiException("Hearing id and participant id not found", HttpStatusCode.NotFound));

            var response = await Controller.UpdateSuitabilityAnswersAsync(Guid.NewGuid(), Guid.NewGuid(), new List<SuitabilityAnswersRequest>());
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Should_return_bad_request_for_invalid_request()
        {
            BookingsApiClient
                .Setup(x => x.UpdateSuitabilityAnswersAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<List<SuitabilityAnswersRequest>>()))
                .ThrowsAsync(CreateBookingsApiException("Request invalid", HttpStatusCode.BadRequest));

            var response = await Controller.UpdateSuitabilityAnswersAsync(Guid.NewGuid(), Guid.NewGuid(), new List<SuitabilityAnswersRequest>());
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}
