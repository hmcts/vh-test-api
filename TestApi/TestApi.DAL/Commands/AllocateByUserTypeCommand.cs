using System.Threading.Tasks;
using TestApi.DAL.Commands.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.DAL.Commands
{
    public class AllocateByUserTypeCommand : ICommand
    {
        public UserType UserType { get; set; }
        public Application Application { get; set; }
        public int ExtendedExpiryInMinutes { get; set; }
        public User User { get; set; }

        public AllocateByUserTypeCommand(UserType userType, Application application, int extendedExpiryInMinutes = 10)
        {
            UserType = userType;
            Application = application;
            ExtendedExpiryInMinutes = extendedExpiryInMinutes;
        }
    }

    public class AllocateByUserTypeCommandHandler : ICommandHandler<AllocateByUserTypeCommand>
    {
        private readonly TestApiDbContext _context;
        private readonly IAllocationService _service;

        public AllocateByUserTypeCommandHandler(TestApiDbContext context, IAllocationService service)
        {
            _context = context;
            _service = service;
        }

        public async Task Handle(AllocateByUserTypeCommand command)
        {
            var user = await _service.AllocateToService(command.UserType, command.Application, command.ExtendedExpiryInMinutes);
            await _context.SaveChangesAsync();
            command.User = user;
        }
    }
}
