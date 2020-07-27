using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Exceptions;

namespace TestApi.DAL.Commands
{
    public class DeleteUserByUserIdCommand : ICommand
    {
        public Guid UserId { get; }

        public DeleteUserByUserIdCommand(Guid userId)
        {
            UserId = userId;
        }
    }

    public class DeleteUserByUserIdCommandHandler : ICommandHandler<DeleteUserByUserIdCommand>
    {
        private readonly TestApiDbContext _context;

        public DeleteUserByUserIdCommandHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteUserByUserIdCommand command)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == command.UserId);

            if (user == null)
            {
                throw new UserNotFoundException(command.UserId);
            }

            _context.Remove(user);

            await _context.SaveChangesAsync();
        }
    }
}
