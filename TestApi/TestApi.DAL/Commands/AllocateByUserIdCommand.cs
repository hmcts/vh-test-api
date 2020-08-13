using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Exceptions;
using TestApi.Domain;

namespace TestApi.DAL.Commands
{
    public class AllocateByUserIdCommand : ICommand
    {
        public AllocateByUserIdCommand(Guid userId, int extendedExpiryInMinutes = 10)
        {
            UserId = userId;
            ExtendedExpiryInMinutes = extendedExpiryInMinutes;
        }

        public Guid UserId { get; set; }
        public int ExtendedExpiryInMinutes { get; set; }
        public User User { get; set; }
    }

    public class AllocateByUserIdCommandHandler : ICommandHandler<AllocateByUserIdCommand>
    {
        private readonly TestApiDbContext _context;

        public AllocateByUserIdCommandHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AllocateByUserIdCommand command)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == command.UserId);

            if (user == null) throw new UserNotFoundException(command.UserId);

            var allocation = await _context.Allocations.SingleOrDefaultAsync(x => x.User.Id == user.Id);

            if (allocation == null) throw new UserAllocationNotFoundException(command.UserId);

            if (allocation.IsAllocated()) throw new UserUnavailableException();

            allocation.Allocate(command.ExtendedExpiryInMinutes);
            await _context.SaveChangesAsync();
            command.User = user;
        }
    }
}