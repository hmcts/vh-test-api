using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.AcceptanceTests.Helpers
{
    public class TestData
    {
        public HearingDetailsResponse Hearing { get; set; }
        public ConferenceDetailsResponse Conference { get; set; }
    }
}
