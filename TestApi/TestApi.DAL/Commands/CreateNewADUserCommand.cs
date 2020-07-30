using System.Threading.Tasks;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Exceptions;
using TestApi.Domain;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Contracts;

namespace TestApi.DAL.Commands
{
    public class CreateNewADUserCommand : ICommand
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleNames { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string ContactEmail { get; set; }
        public string CaseRoleName { get; set; }
        public string HearingRoleName { get; set; }
        public string Reference { get; set; }
        public string Representee { get; set; }
        public string OrganisationName { get; set; }
        public string TelephoneNumber { get; set; }
        public NewUserResponse Response { get; set; }

        public CreateNewADUserCommand(string title, string firstName, string middleNames, string lastName,
            string displayName, string username, string contactEmail, string caseRoleName,
            string hearingRoleName, string reference, string representee, string organisation,
            string telephoneNumber)
        {
            Title = title;
            FirstName = firstName;
            MiddleNames = middleNames;
            LastName = lastName;
            DisplayName = displayName;
            Username = username;
            ContactEmail = contactEmail;
            CaseRoleName = caseRoleName;
            HearingRoleName = hearingRoleName;
            Reference = reference;
            Representee = representee;
            OrganisationName = organisation;
            TelephoneNumber = telephoneNumber;
        }
    }

    public class CreateNewADUserCommandHandler : ICommandHandler<CreateNewADUserCommand>
    {
        private readonly IUserApiService _service;

        public CreateNewADUserCommandHandler(IUserApiService service)
        {
            _service = service;
        }

        public async Task Handle(CreateNewADUserCommand command)
        {
            var adUser = new ADUser()
            {
                Title = command.Title,
                FirstName = command.FirstName,
                MiddleNames = command.MiddleNames,
                LastName = command.LastName,
                DisplayName = command.DisplayName,
                Username = command.Username,
                ContactEmail = command.ContactEmail,
                CaseRoleName = command.CaseRoleName,
                HearingRoleName = command.HearingRoleName,
                Reference = command.Reference,
                Representee = command.Representee,
                OrganisationName = command.OrganisationName,
                TelephoneNumber = command.TelephoneNumber
            };

            var userExists = await _service.CheckUserExistsInAAD(command.ContactEmail);

            if (userExists)
            {
                throw new UserAlreadyExistsException(command.ContactEmail);
            }

            command.Response = await _service.CreateNewUserInAAD(adUser);
        }
    }
}
