using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Domain;
using TestApi.Contract.Enums;
using TestApi.Domain.Validations;

namespace TestApi.UnitTests.Domain
{
    public class AllocationDomainTests
    {
        private Allocation _allocation;
        private User _user;

        [SetUp]
        public void SetUp()
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;
            const int NUMBER = 1;

            _user = new UserBuilder(EMAIL_STEM, NUMBER)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .BuildUser();

            _allocation = new Allocation(_user);
        }

        [Test]
        public void Should_set_default_values()
        {
            _allocation.UserId.Should().Be(_user.Id);
            _allocation.Username.Should().Be(_user.Username);
            _allocation.Allocated.Should().BeFalse();
            _allocation.ExpiresAt.Should().BeNull();
        }

        [Test]
        public void Should_allocate_user()
        {
            const int MINUTES = 5;
            _allocation.Allocate(MINUTES);
            _allocation.Allocated.Should().BeTrue();
            _allocation.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(MINUTES), TimeSpan.FromSeconds(5));
            _allocation.IsAllocated().Should().BeTrue();
        }

        [Test]
        public void Should_unallocate_user()
        {
            const int MINUTES = 5;
            _allocation.Allocate(MINUTES);
            _allocation.Unallocate();
            _allocation.Allocated.Should().BeFalse();
            _allocation.ExpiresAt.Should().BeNull();
            _allocation.IsAllocated().Should().BeFalse();
        }

        [Test]
        public void Should_return_is_allocated_false_for_expired_allocated_user()
        {
            const int MINUTES = 0;
            _allocation.Allocate(MINUTES);
            _allocation.Allocated.Should().BeTrue();
            _allocation.ExpiresAt.Should().BeBefore(DateTime.UtcNow);
            _allocation.IsAllocated().Should().BeFalse();
        }

        [Test]
        public void Should_not_allocate_allocated_user()
        {
            const int MINUTES = 5;
            const int SECOND_TRY_MINUTES = 10;
            _allocation.Allocate(MINUTES);

            Action action = () => _allocation.Allocate(SECOND_TRY_MINUTES);
            action.Should().Throw<DomainRuleException>().And.ValidationFailures
                .Any(x => x.Message == "User is already allocated").Should().BeTrue();

            _allocation.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(MINUTES), TimeSpan.FromSeconds(5));
        }

        [Test]
        public void Should_allocate_previously_allocated_user()
        {
            const int MINUTES = 5;
            const int SECOND_MINUTES = 10;
            _allocation.Allocate(MINUTES);
            _allocation.Unallocate();
            _allocation.Allocate(SECOND_MINUTES);
            _allocation.ExpiresAt.Should()
                .BeCloseTo(DateTime.UtcNow.AddMinutes(SECOND_MINUTES), TimeSpan.FromSeconds(5));
        }
    }
}