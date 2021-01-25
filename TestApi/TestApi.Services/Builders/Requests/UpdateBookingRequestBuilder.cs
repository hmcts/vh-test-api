using TestApi.Common.Data;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Builders.Requests
{
    public class UpdateBookingRequestBuilder
    {
        private readonly UpdateBookingStatusRequest _request;

        public UpdateBookingRequestBuilder()
        {
            _request = new UpdateBookingStatusRequest
            {
                Status = UpdateBookingStatus.Created, Updated_by = UserData.DEFAULT_UPDATED_BY_USER
            };
        }

        public UpdateBookingRequestBuilder UpdatedBy(string username)
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