using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Validations;

namespace TestApi.UnitTests.Validations
{
    public class EmailValidationTests
    {
        [Test]
        public void Should_pass_validation_with_good_email()
        {
            const string EMAIL = EmailData.EXISTING_CONTACT_EMAIL;
            EMAIL.IsValidEmail().Should().BeTrue();
        }

        [Test]
        public void Should_fail_validation_when_empty()
        {
            var EMAIL = string.Empty;
            EMAIL.IsValidEmail().Should().BeFalse();
        }

        [Test]
        public void Should_fail_validation_when_format_is_invalid()
        {
            const string EMAIL = "invalid_email_format";
            EMAIL.IsValidEmail().Should().BeFalse();
        }
    }
}