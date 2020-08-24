using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Exceptions;

namespace TestApi.DAL.Commands
{
    public class UnallocateByUsernameCommand : ICommand
    {
        public UnallocateByUsernameCommand(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }

    public class UnallocateUserCommandHandler : ICommandHandler<UnallocateByUsernameCommand>
    {
        private readonly TestApiDbContext _context;

        public UnallocateUserCommandHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UnallocateByUsernameCommand command)
        {
            var user = await _context.Users.SingleOrDefaultAsync(
                x => x.Username.ToLower() == command.Username.ToLower());

            if (user == null) throw new UserNotFoundException(command.Username);

            var allocation = await _context.Allocations.SingleOrDefaultAsync(x => x.UserId == user.Id);

            if (allocation == null) throw new UserAllocationNotFoundException(command.Username);

            allocation.Unallocate();
            await _context.SaveChangesAsync();
        }
    }
}