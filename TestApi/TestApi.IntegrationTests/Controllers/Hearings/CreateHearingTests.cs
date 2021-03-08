using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Contract.Enums;
using TestApi.Tests.Common;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class CreateHearingTests : HearingsTestsBase
    {
        [TestCase(TestType.Automated)]
        [TestCase(TestType.ITHC)]
        [TestCase(TestType.Manual)]
        [TestCase(TestType.Performance)]
        public async Task Should_create_hearing(TestType testType)
        {
            var request = CreateHearingRequest(testType);
            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
        }

        [Test]
        public async Task Should_create_CACD_hearing()
        {
            var request = CreateCACDHearingRequest();
            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
        }

        [Test]
        public async Task Should_create_hearing_with_several_endpoints()
        {
            const int ENDPOINTS_COUNT = 2;
            var request = CreateHearingRequest();
            request.Endpoints = ENDPOINTS_COUNT;

            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
            response.Endpoints.Count.Should().Be(ENDPOINTS_COUNT);
            response.Endpoints.All(x => x.DisplayName.StartsWith(HearingData.ENDPOINT_PREFIX)).Should().BeTrue();
        }

        [Test]
        public async Task Should_create_hearing_with_more_individuals_than_reps()
        {
            var request = CreateHearingWithJustIndividualAndJudge();
            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
        }

        [Test]
        public async Task Should_create_hearing_with_more_representatives_than_reps()
        {
            var request = CreateHearingWithJustRepAndJudge();
            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
            response.Participants.Single(x => x.LastName.Contains("Representative")).Representee.Should()
                .Be(HearingData.REPRESENTEE);
        }

        [Test]
        public async Task Should_create_hearing_with_custom_case_name_prefix()
        {
            const string CUSTOM_CASE_NAME = "A Custom case name prefix";
            var request = CreateHearingRequest();
            request.CustomCaseNamePrefix = CUSTOM_CASE_NAME;
            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.HearingDetailsResponse(response, request);
            response.Cases.Single().Name.Should().StartWith(CUSTOM_CASE_NAME);
        }

        [Test]
        public async Task Should_create_hearing_with_created_by()
        {
            const string CREATED_BY = "A_Custom_Created_By_User@hmcts.net";
            var request = CreateHearingRequest();
            request.CreatedBy = CREATED_BY;
            await CreateHearing(request);

            VerifyResponse(HttpStatusCode.Created, true);
            var response = RequestHelper.Deserialise<HearingDetailsResponse>(Json);

            response.Should().NotBeNull();
            response.CreatedBy.Should().Be(CREATED_BY);
        }
    }
}
