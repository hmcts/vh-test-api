using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Exceptions;
using TestApi.Domain;

namespace TestApi.DAL.Commands
{
    public class CreateNewRecentUserByUsernameCommand : ICommand
    {
        public CreateNewRecentUserByUsernameCommand(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }

    public class CreateNewRecentUserByUsernameCommandHandler : ICommandHandler<CreateNewRecentUserByUsernameCommand>
    {
        private readonly TestApiDbContext _context;

        public CreateNewRecentUserByUsernameCommandHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateNewRecentUserByUsernameCommand command)
        {
            var recentUser = await _context.RecentUsers
                .SingleOrDefaultAsync(x => x.Username == command.Username);

            if (recentUser != null) throw new RecentUserAlreadyExistsException(command.Username);

            recentUser = new RecentUser(command.Username);

            await _context.RecentUsers.AddAsync(recentUser);
            await _context.SaveChangesAsync();
        }
    }
}