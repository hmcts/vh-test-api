using System.Collections.Generic;
using TestApi.Contract.Responses;
using TestApi.Domain;

namespace TestApi.IntegrationTests.Data
{
    public class TestData
    {
        public Allocation Allocation { get; set; }
        public List<Allocation> Allocations { get; set; }
        public User User { get; set; }
        public List<User> Users { get; set; }
        public UserDetailsResponse UserDetailsResponse { get; set; }
        public List<UserDetailsResponse> UserResponses { get; set; }
    }
}
