using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;
using TestApi.Common.Data;

namespace TestApi.Services.Builders.Requests
{
    public class UpdateBookingRequestBuilder
    {
        private readonly UpdateBookingStatusRequest _request;

        public UpdateBookingRequestBuilder()
        {
            _request = new UpdateBookingStatusRequest
            {
                Status = UpdateBookingStatus.Created, UpdatedBy = UserData.DEFAULT_UPDATED_BY_USER
            };
        }

        public UpdateBookingRequestBuilder UpdatedBy(string username)
        {
            _request.UpdatedBy = username;
            return this;
        }

        public UpdateBookingStatusRequest Build()
        {
            return _request;
        }
    }
}