using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Exceptions;

namespace TestApi.DAL.Commands
{
    public class DeleteNewRecentUserByUsernameCommand : ICommand
    {
        public DeleteNewRecentUserByUsernameCommand(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }

    public class DeleteNewRecentUserByUsernameCommandHandler : ICommandHandler<DeleteNewRecentUserByUsernameCommand>
    {
        private readonly TestApiDbContext _context;

        public DeleteNewRecentUserByUsernameCommandHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteNewRecentUserByUsernameCommand command)
        {
            var recentUser = await _context.RecentUsers
                .SingleOrDefaultAsync(x => x.Username == command.Username);

            if (recentUser == null) throw new RecentUserNotFoundException(command.Username);

            _context.Remove(recentUser);
            await _context.SaveChangesAsync();
        }
    }
}