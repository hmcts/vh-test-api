using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Contract.Requests;
using TestApi.Validations;

namespace TestApi.UnitTests.Validations
{
    public class UnallocateUsersValidationTests
    {
        private const string USERNAME = "user@hmcts.net";
        private UnallocateUsersRequestValidator _validator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _validator = new UnallocateUsersRequestValidator();
        }

        [Test]
        public async Task Should_pass_validation()
        {
            var request = new UnallocateUsersRequest
            {
                Usernames = new List<string> {USERNAME}
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_empty_usernames_error()
        {
            var request = new UnallocateUsersRequest
            {
                Usernames = new List<string>()
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().ErrorMessage.Should()
                .Be(UnallocateUsersRequestValidator.EMPTY_USERNAMES_ERROR_MESSAGE);
        }
    }
}