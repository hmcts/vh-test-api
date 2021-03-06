﻿using System;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class GetHearingByIdControllerTests : HearingsControllerTestsBase
    {
        [Test]
        public async Task Should_return_hearing()
        {
            var hearingDetailsResponse = CreateHearingDetailsResponse();

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(hearingDetailsResponse.Id))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.GetHearingById(hearingDetailsResponse.Id);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var hearingDetails = (HearingDetailsResponse) result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing()
        {
            var nonExistentHearingId = Guid.NewGuid();

            BookingsApiClient
                .Setup(x => x.GetHearingDetailsByIdAsync(nonExistentHearingId))
                .ThrowsAsync(CreateBookingsApiException("Hearing not found", HttpStatusCode.NotFound));

            var response = await Controller.GetHearingById(nonExistentHearingId);
            response.Should().NotBeNull();

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }
    }
}