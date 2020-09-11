using System;
using TestApi.Domain.Ddd;

namespace TestApi.Domain
{
    public class RecentUser : Entity<Guid>
    {
        public RecentUser(string username)
        {
            Username = username;
            CreatedAt = DateTime.UtcNow;
        }

        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsRecentlyCreated()
        {
            return CreatedAt.AddMinutes(1) > DateTime.UtcNow;
        }
    }
}
