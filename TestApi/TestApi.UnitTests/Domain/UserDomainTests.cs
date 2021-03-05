using System;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Enums;

namespace TestApi.UnitTests.Domain
{
    public class UserDomainTests
    {
        [Test]
        public void Should_set_created_date()
        {
            const string emailStem = EmailData.FAKE_EMAIL_STEM;
            const int number = 1;

            var user = new UserBuilder(emailStem, number)
                .WithUserType(UserType.Judge)
                .ForApplication(Application.TestApi)
                .BuildUserDto();

            user.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }
    }
}