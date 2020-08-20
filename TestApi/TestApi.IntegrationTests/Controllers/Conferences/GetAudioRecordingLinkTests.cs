using System;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.AudioRecordings;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Conferences
{
    public class GetAudioRecordingLinkTests : ConferencesTestsBase
    {
        private AzureStorageManager _azureStorage;
        
        private async Task CreateAudioFileInWowza(Guid hearingRefId)
        {
            var file = FileManager.CreateNewAudioFile("TestAudioFile.mp4", hearingRefId);

            _azureStorage = new AzureStorageManager()
                .SetStorageAccountName(Context.Config.Wowza.StorageAccountName)
                .SetStorageAccountKey(Context.Config.Wowza.StorageAccountKey)
                .SetStorageContainerName(Context.Config.Wowza.StorageContainerName)
                .CreateBlobClient(hearingRefId);

            await _azureStorage.UploadAudioFileToStorage(file);
            FileManager.RemoveLocalAudioFile(file);
        }

        [Test]
        public async Task Should_get_audio_recording_link()
        {
            var request = CreateConferenceRequest();
            var conference = await CreateConference(request);
            await CreateAudioFileInWowza(conference.Hearing_id);

            var uri = ApiUriFactory.ConferenceEndpoints.GetAudioRecordingLinkByHearingId(conference.Hearing_id);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<AudioRecordingResponse>(Json);

            response.Should().NotBeNull();
            response.Audio_file_link.Should().Contain(conference.Hearing_id.ToString());
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_audio_recording_link()
        {
            var uri = ApiUriFactory.ConferenceEndpoints.GetAudioRecordingLinkByHearingId(Guid.NewGuid());
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.NotFound, false);
        }

        [OneTimeTearDown]
        public void RemoveAudioFileFromWowza()
        {
            _azureStorage?.RemoveAudioFileFromStorage();
        }
    }
}
