using System.Threading.Tasks;
using TestApi.DAL.Commands.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.DAL.Commands
{
    public class AllocateByUserTypeAndApplicationCommand : ICommand
    {
        public UserType UserType { get; set; }
        public Application Application { get; set; }
        public int ExtendedExpiryInMinutes { get; set; }
        public User User { get; set; }

        public AllocateByUserTypeAndApplicationCommand(UserType userType, Application application, int extendedExpiryInMinutes = 10)
        {
            UserType = userType;
            Application = application;
            ExtendedExpiryInMinutes = extendedExpiryInMinutes;
        }
    }

    public class AllocateByUserTypeAndApplicationCommandHandler : ICommandHandler<AllocateByUserTypeAndApplicationCommand>
    {
        private readonly TestApiDbContext _context;
        private readonly IAllocationService _service;

        public AllocateByUserTypeAndApplicationCommandHandler(TestApiDbContext context, IAllocationService service)
        {
            _context = context;
            _service = service;
        }

        public async Task Handle(AllocateByUserTypeAndApplicationCommand command)
        {
            var user = await _service.AllocateToService(command.UserType, command.Application, command.ExtendedExpiryInMinutes);
            await _context.SaveChangesAsync();
            command.User = user;
        }
    }
}
