using System;
using TestApi.Contract.Enums;

namespace TestApi.Contract.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
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
