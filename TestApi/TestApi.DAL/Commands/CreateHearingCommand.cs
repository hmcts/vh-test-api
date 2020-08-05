using System.Threading.Tasks;
using TestApi.Contract.Requests;
using TestApi.DAL.Commands.Core;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Contracts;

namespace TestApi.DAL.Commands
{
    public class CreateHearingCommand : ICommand
    {
        public CreateHearingRequest Request { get; set; }
        public HearingDetailsResponse Response { get; set; }

        public CreateHearingCommand(CreateHearingRequest request)
        {
            Request = request;
        }
    }

    public class CreateHearingCommandHandler : ICommandHandler<CreateHearingCommand>
    {
        private readonly TestApiDbContext _context;
        private readonly IBookingsApiService _service;

        public CreateHearingCommandHandler(TestApiDbContext context, IBookingsApiService service)
        {
            _context = context;
            _service = service;
        }

        public async Task Handle(CreateHearingCommand command)
        {
            var hearing = await _service.CreateHearing(command.Request);
            command.Response = hearing;
        }
    }
}
