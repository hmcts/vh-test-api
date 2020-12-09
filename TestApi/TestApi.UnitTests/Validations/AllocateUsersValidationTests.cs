using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Domain.Enums;
using TestApi.Validations;

namespace TestApi.UnitTests.Validations
{
    public class AllocateUsersValidationTests
    {
        private AllocateUsersRequestValidator _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new AllocateUsersRequestValidator();
        }

        [Test]
        public async Task Should_pass_validation_with_case_admin()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithDefaultTypes()
                .ForApplication(Application.TestApi)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_pass_validation_with_one_vho()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithVideoHearingsOfficer()
                .ForApplication(Application.TestApi)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Test]
        public async Task Should_return_more_than_one_joh_error()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithMoreThanOneJoh()
                .ForApplication(Application.TestApi)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUsersRequestValidator.MORE_THAN_ONE_JOH_ERROR_MESSAGE);
        }

        [Test]
        public async Task Should_return_empty_users_error()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithEmptyUsers()
                .ForApplication(Application.TestApi)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().BeGreaterThan(0);
            result.Errors.Any(x => x.ErrorMessage == AllocateUsersRequestValidator.EMPTY_USERS_ERROR_MESSAGE)
                .Should().BeTrue();
        }

        [Test]
        public async Task Should_return_empty_application_error()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithDefaultTypes()
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUsersRequestValidator.EMPTY_APPLICATION_ERROR_MESSAGE);
        }

        [Test]
        public async Task Should_return_missing_admin_users()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithoutCaseAdmin()
                .ForApplication(Application.TestApi)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUsersRequestValidator.MISSING_ADMIN_USERS_ERROR_MESSAGE);
        }

        [Test]
        public async Task Should_return_more_than_one_judge_error()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithMoreThanOneJudge()
                .ForApplication(Application.TestApi)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUsersRequestValidator.MORE_THAN_ONE_JUDGE_ERROR_MESSAGE);
        }

        [Test]
        public async Task Should_be_greater_than_zero_for_expiry()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithDefaultTypes()
                .ForApplication(Application.TestApi)
                .WithExpiryInMinutes(-1)
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUsersRequestValidator.EXPIRES_IN_GREATER_THAN_ZERO_ERROR_MESSAGE);
        }

        [Test]
        public async Task Should_be_less_than_twelve_hours_for_expiry()
        {
            var request = new AllocateUsersRequestBuilder()
                .WithDefaultTypes()
                .ForApplication(Application.TestApi)
                .WithExpiryInMinutes(Convert.ToInt32(TimeSpan.FromHours(12).Add(TimeSpan.FromMinutes(1)).TotalMinutes))
                .Build();

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(AllocateUsersRequestValidator.EXPIRES_IN_LESS_THAN_TWELVE_HOURS_ERROR_MESSAGE);
        }
    }
}