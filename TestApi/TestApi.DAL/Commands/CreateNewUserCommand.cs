using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.Common.Extensions;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Exceptions;
using TestApi.Domain;
using TestApi.Contract.Enums;

namespace TestApi.DAL.Commands
{
    public class CreateNewUserCommand : ICommand
    {
        public CreateNewUserCommand(string username, string contactEmail, string firstName, string lastName, string displayName, 
            int number, TestType testType, UserType userType, Application application, bool isProdUser)
        {
            Username = username;
            ContactEmail = contactEmail;
            FirstName = firstName;
            LastName = lastName;
            DisplayName = displayName;
            Number = number;
            TestType = testType;
            UserType = userType;
            Application = application;
            IsProdUser = isProdUser;
        }

        public string Username { get; set; }
        public string ContactEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public int Number { get; set; }
        public TestType TestType { get; set; }
        public UserType UserType { get; set; }
        public Application Application { get; set; }
        public bool IsProdUser { get; set; }
        public Guid NewUserId { get; set; }
    }

    public class CreateNewUserCommandHandler : ICommandHandler<CreateNewUserCommand>
    {
        private readonly TestApiDbContext _context;

        public CreateNewUserCommandHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateNewUserCommand command)
        {
            var userWithNumberExistsAlready = await _context.Users
                .Where(x => x.UserType == command.UserType.MapToContractEnum() && x.Application == command.Application.MapToContractEnum() && x.Number == command.Number)
                .AsNoTracking()
                .AnyAsync();

            if (userWithNumberExistsAlready)
                throw new MatchingUserWithNumberExistsException(command.UserType.MapToContractEnum(), command.Number);

            var user = new User(
                command.Username,
                command.ContactEmail,
                command.FirstName,
                command.LastName,
                command.DisplayName,
                command.Number,
                command.TestType.MapToContractEnum(),
                command.UserType.MapToContractEnum(),
                command.Application.MapToContractEnum(),
                command.IsProdUser
            );

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            command.NewUserId = user.Id;
        }
    }
}