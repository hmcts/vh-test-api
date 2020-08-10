using System;
using TestApi.Domain.Enums;

namespace TestApi.IntegrationTests.Configuration
{
    public static class ApiUriFactory
    {
        public static class AllocationEndpoints
        {
            public const string ApiRoot = "allocations";
            public static string AllocateByUserTypeAndApplication(UserType userType, Application application) => $"{ApiRoot}/?userType={userType}&application={application}";
            public static string AllocateUsers => $"{ApiRoot}/allocateUsers";
            public static string UnallocateUsers => $"{ApiRoot}/unallocateUsers";
        }

        public static class HealthCheckEndpoints
        {
            private const string ApiRoot = "/healthCheck";
            public static string CheckServiceHealth => $"{ApiRoot}/health";
        }

        public static class HearingEndpoints
        {
            private const string ApiRoot = "/hearings";
            public static string GetHearing(Guid hearingId) => $"{ApiRoot}/{hearingId:D}";
            public static string CreateHearing => ApiRoot;
            public static string ConfirmHearing(Guid hearingId) => $"{ApiRoot}/{hearingId:D}";
            public static string DeleteHearing(Guid hearingId) => $"{ApiRoot}/{hearingId:D}";
        }

        public static class UserEndpoints
        {
            public const string ApiRoot = "users";
            public static string GetUserByUsername(string username) => $"{ApiRoot}/username/{username}";
            public static string DeleteAADUser => $"{ApiRoot}/aad";
        }
    }
}
