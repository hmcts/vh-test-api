using System;
using TestApi.Domain.Enums;

namespace TestApi.IntegrationTests.Configuration
{
    public static class ApiUriFactory
    {
        public static class AllocationEndpoints
        {
            public const string ApiRoot = "allocations";
            public static string GetAllocationByUserId(Guid userId) => $"{ApiRoot}/{userId:D}";
            public static string AllocateByUserId(Guid userId) => $"{ApiRoot}/{userId:D}";
            public static string AllocateByUserTypeAndApplication(UserType userType, Application application) => $"{ApiRoot}/?userType={userType}&application={application}";
            public static string CreateAllocation(Guid userId) => $"{ApiRoot}/?userId={userId:D}";
            public static string DeleteAllocation(Guid userId) => $"{ApiRoot}/?userId={userId:D}";
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
            public static string GetUserById(Guid userId) => $"{ApiRoot}/{userId:D}";
            public static string GetUserByUsername(string username) => $"{ApiRoot}/username/{username}";
            public static string GetAllUsersByUserTypeAndApplication(UserType userType, Application application) => $"{ApiRoot}/?userType={userType}&application={application}";
            public static string GetIteratedUserNumber(UserType userType, Application application) => $"{ApiRoot}/iterate/?userType={userType}&application={application}";
            public static string CreateUser => ApiRoot;
            public static string DeleteUser(Guid userId) => $"{ApiRoot}/?userId={userId:D}";
            public static string CreateAADUser => $"{ApiRoot}/aad";
        }
    }
}
