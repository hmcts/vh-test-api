using System;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Domain.Enums;

namespace TestApi.UnitTests.Domain
{
    public class UserDomainTests
    {
        [Test]
        public void Should_set_created_date()
        {
            const string emailStem = "made_up_email_stem";
            const int number = 1;

            var user = new UserBuilder(emailStem, number)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .BuildUser();

            user.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }
    }
}