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
            Verify.UserDetailsResponse(userDetailsResponse, USER_TYPE);
        }
    }
}
