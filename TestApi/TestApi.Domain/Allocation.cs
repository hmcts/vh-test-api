using System;
using TestApi.Domain.Ddd;
using TestApi.Domain.Validations;

namespace TestApi.Domain
{
    public class Allocation : Entity<Guid>
    {
        protected Allocation()
        {
            ExpiresAt = null;
            Allocated = false;
        }

        public Allocation(User user) : this()
        {
            UserId = user.Id;
            Username = user.Username;
        }

        public virtual User User { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool Allocated { get; set; }

        public void Allocate(int minutes)
        {
            if (IsAllocated()) throw new DomainRuleException("Allocation", "User is already allocated");

            Allocated = true;
            ExpiresAt = DateTime.UtcNow.AddMinutes(minutes);
        }

        public bool IsAllocated()
        {
            if (ExpiresAt == null) return false;

            return Allocated && DateTime.UtcNow < ExpiresAt;
        }

        public void Unallocate()
        {
            Allocated = false;
            ExpiresAt = null;
        }
    }
}