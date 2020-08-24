using System;
using TestApi.Domain.Ddd;
using TestApi.Domain.Enums;

namespace TestApi.Domain
{
    public class User : AggregateRoot<Guid>
    {
        public User()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public User(string username, string contactEmail, string firstName, string lastName, string displayName, 
            int number, TestType testType, UserType userType, Application application, bool isProdUser) : this()
        {
            Id = Guid.NewGuid();
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
        public DateTime CreatedDate { get; set; }
    }
}