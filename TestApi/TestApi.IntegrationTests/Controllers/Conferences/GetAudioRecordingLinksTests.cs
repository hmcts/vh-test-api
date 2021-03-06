using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.AudioRecordings;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Tests.Common.Configuration;
using VideoApi.Contract.Responses;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class GetAudioRecordingLinksTests : ConferencesTestsBase
    {
        private AzureStorageManager _azureStorage;
        
        private async Task CreateAudioFileInWowza(Guid hearingRefId)
        {
            var file = FileManager.CreateNewAudioFile("TestAudioFile.mp4", hearingRefId.ToString());

            _azureStorage = new AzureStorageManager()
                .SetStorageAccountName(Context.Config.Wowza.StorageAccountName)
                .SetStorageAccountKey(Context.Config.Wowza.StorageAccountKey)
                .SetStorageContainerName(Context.Config.Wowza.StorageContainerName)
                .CreateBlobClient(hearingRefId.ToString());

            await _azureStorage.UploadAudioFileToStorage(file);
            FileManager.RemoveLocalAudioFile(file);
        }

        [Test]
        public async Task Should_get_audio_recording_links()
        {
            var request = CreateConferenceRequest();
            var conference = await CreateConference(request);
            await CreateAudioFileInWowza(conference.HearingId);

            var uri = ApiUriFactory.ConferenceEndpoints.GetAudioRecordingLinksByHearingId(conference.HearingId);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<AudioRecordingResponse>(Json);

            response.Should().NotBeNull();
            response.AudioFileLinks.First().Should().Contain(conference.HearingId.ToString());
        }

        [Test]
        public async Task Should_return_ok_for_non_existent_audio_recording_links()
        {
            var uri = ApiUriFactory.ConferenceEndpoints.GetAudioRecordingLinksByHearingId(Guid.NewGuid());
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<AudioRecordingResponse>(Json);
            response.AudioFileLinks.Count.Should().Be(0);
        }

        [OneTimeTearDown]
        public void RemoveAudioFileFromWowza()
        {
            _azureStorage?.RemoveAudioFileFromStorage();
        }
    }
}
