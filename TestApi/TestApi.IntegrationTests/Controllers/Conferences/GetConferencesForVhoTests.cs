﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class GetConferencesForVhoTests : ConferencesTestsBase
    {
        [Test]
        public async Task Should_get_conferences_for_a_vho()
        {
            var request = CreateConferenceRequest();
            await CreateConference(request);
            
            var uri = ApiUriFactory.ConferenceEndpoints.GetConferencesForVho;

            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<ConferenceForAdminResponse>>(Json);

            response.Should().NotBeNull();
            Verify.ConferencesForVhoResponses(response, request);
        }
    }
}
