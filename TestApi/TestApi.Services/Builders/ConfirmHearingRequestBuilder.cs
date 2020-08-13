using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Builders
{
    public class ConfirmHearingRequestBuilder
    {
        private readonly UpdateBookingStatusRequest _request;

        public ConfirmHearingRequestBuilder()
        {
            _request = new UpdateBookingStatusRequest {Status = UpdateBookingStatus.Created};
        }

        public ConfirmHearingRequestBuilder UpdatedBy(string username)
        {
            _request.Updated_by = username;
            return this;
        }

        public UpdateBookingStatusRequest Build()
        {
            return _request;
        }
    }
}