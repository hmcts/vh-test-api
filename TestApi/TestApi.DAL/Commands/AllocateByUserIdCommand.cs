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
        public AllocateByUserIdCommand(Guid userId, int extendedExpiryInMinutes = 10, string allocatedBy = null)
        {
            UserId = userId;
            ExtendedExpiryInMinutes = extendedExpiryInMinutes;
            AllocatedBy = allocatedBy;
        }

        public Guid UserId { get; set; }
        public int ExtendedExpiryInMinutes { get; set; }
        public User User { get; set; }
        public string AllocatedBy { get; set; }
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

            var allocation = await _context.Allocations.SingleOrDefaultAsync(x => x.UserId == user.Id);

            if (allocation == null) throw new UserAllocationNotFoundException(command.UserId);

            if (allocation.IsAllocated()) throw new UserUnavailableException();

            allocation.Allocate(command.ExtendedExpiryInMinutes, command.AllocatedBy);
            await _context.SaveChangesAsync();
            command.User = user;
        }
    }
}