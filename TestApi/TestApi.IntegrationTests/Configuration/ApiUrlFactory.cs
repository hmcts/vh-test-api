using System;
using TestApi.Domain.Enums;

namespace TestApi.IntegrationTests.Configuration
{
    public static class ApiUriFactory
    {
        public static class AllocationEndpoints
        {
            public const string ApiRoot = "allocations";
            public static string AllocateUsers => $"{ApiRoot}/allocateUsers";
            public static string UnallocateUsers => $"{ApiRoot}/unallocateUsers";

            public static string AllocateSingleUser(UserType userType, Application application)
            {
                return $"{ApiRoot}/?userType={userType}&application={application}";
            }
        }

        public static class HealthCheckEndpoints
        {
            private const string ApiRoot = "/healthCheck";
            public static string CheckServiceHealth => $"{ApiRoot}/health";
        }

        public static class HearingEndpoints
        {
            private const string ApiRoot = "/hearings";
            public static string CreateHearing => ApiRoot;

            public static string GetHearing(Guid hearingId)
            {
                return $"{ApiRoot}/{hearingId:D}";
            }

            public static string ConfirmHearing(Guid hearingId)
            {
                return $"{ApiRoot}/{hearingId:D}";
            }

            public static string DeleteHearing(Guid hearingId)
            {
                return $"{ApiRoot}/{hearingId:D}";
            }
        }

        public static class UserEndpoints
        {
            public const string ApiRoot = "users";

            public static string GetUserByUsername(string username)
            {
                return $"{ApiRoot}/username/{username}";
            }

            public static string DeleteAADUser(string username)
            {
                return $"{ApiRoot}/aad/{username}";
            }
        }
    }
}