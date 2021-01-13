using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Validations;

namespace TestApi.UnitTests.Validations
{
    public class AllocateUserValidationTests
    {
        private AllocateUserRequestValidator _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AllocateUserRequestValidator();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new AllocateUserRequestBuilder().Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_be_greater_than_zero_for_expiry()
        {
            var request = new AllocateUserRequestBuilder().WithExpiryInMinutes(-1).Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUserRequestValidator.EXPIRES_IN_GREATER_THAN_ZERO_ERROR_MESSAGE);
        }

        [Test]
        public async Task Should_be_less_than_twelve_hours_for_expiry()
        {
            var request = new AllocateUserRequestBuilder()
                .WithExpiryInMinutes(Convert.ToInt32(TimeSpan.FromHours(12).Add(TimeSpan.FromDays(30)).TotalMinutes))
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUserRequestValidator.EXPIRES_IN_LESS_THAN_THIRTY_DAYS_ERROR_MESSAGE);
        }
    }
}