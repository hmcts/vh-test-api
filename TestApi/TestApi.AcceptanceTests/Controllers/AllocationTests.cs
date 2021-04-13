using FluentAssertions;
using NUnit.Framework;
using TestApi.Contract.Enums;
using TestApi.Tests.Common;

namespace TestApi.AcceptanceTests.Controllers
{
    public class AllocationTests : TestsBase
    {
        [Test]
        public void AllocateSingleUser()
        {
            const UserType USER_TYPE = UserType.Individual;
            var userDetailsResponse = AllocateUser(USER_TYPE);
            userDetailsResponse.Username.Should().EndWith(Context.Config.UsernameStem);
            Verify.UserDetailsResponse(userDetailsResponse, USER_TYPE);
        }

        [Test]
        public void AllocateSingleEjudUser()
        {
            const UserType USER_TYPE = UserType.Judge;
            var userDetailsResponse = AllocateUser(USER_TYPE, true);
            userDetailsResponse.ContactEmail.Should().EndWith(Context.Config.EjudUsernameStem);
            userDetailsResponse.Username.Should().EndWith(Context.Config.EjudUsernameStem);
            Verify.EjudUserDetailsResponse(userDetailsResponse, USER_TYPE);
        }
    }
}
