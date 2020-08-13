using System.Threading.Tasks;
using TestApi.Contract.Requests;
using TestApi.Services.Builders;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Contracts
{
    public interface IBookingsApiService
    {
        /// <summary>Creates a hearing </summary>
        /// <returns>Details of the created hearing</returns>
        Task<HearingDetailsResponse> CreateHearing(CreateHearingRequest createHearingRequest);
    }

    public class BookingsApiService : IBookingsApiService
    {
        private readonly IBookingsApiClient _bookingsApiClient;

        public BookingsApiService(IBookingsApiClient bookingsApiClient)
        {
            _bookingsApiClient = bookingsApiClient;
        }

        public async Task<HearingDetailsResponse> CreateHearing(CreateHearingRequest createHearingRequest)
        {
            var request = new BookHearingRequestBuilder(createHearingRequest).Build();
            return await _bookingsApiClient.BookNewHearingAsync(request);
        }
    }
}