using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;
using VideoApi.Contract.Responses;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class GetTasksTests : ConferencesTestsBase
    {
        [Test]
        public async Task Should_get_tasks_by_conference_id()
        {
            var conferenceRequest = CreateConferenceRequest();
            var conference = await CreateConference(conferenceRequest);
            var videoEventRequest = CreateVideoEventRequest(conference);
            await CreateEvent(videoEventRequest);

            var uri = ApiUriFactory.ConferenceEndpoints.GetTasksByConferenceId(conference.Id);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<TaskResponse>>(Json);

            response.Should().NotBeNull();
            Verify.Tasks(response.First(), videoEventRequest);
        }

        [Test]
        public async Task Should_return_empty_list_if_no_tasks_exist()
        {
            var uri = ApiUriFactory.ConferenceEndpoints.GetTasksByConferenceId(Guid.NewGuid());
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<TaskResponse>>(Json);

            response.Should().NotBeNull();
            response.Count.Should().Be(0);
        }
    }
}
