namespace TestApi.Domain
{
    public class ADUser
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

        public ADUser() {}

        public ADUser(string title, string firstName, string middleNames, string lastName,
            string displayName, string username, string contactEmail,  string caseRoleName, 
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
}
