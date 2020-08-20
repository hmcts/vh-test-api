using System;
using TestApi.Domain.Enums;

namespace TestApi.Tests.Common.Configuration
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

        public static class ConferenceEndpoints
        {
            public const string ApiRoot = "/conferences";
            public static string GetConferenceById(Guid conferenceId) => $"{ApiRoot}/{conferenceId:D}";
            public static string GetConferenceByHearingRefId(Guid hearingRefId) => $"{ApiRoot}/hearings/{hearingRefId:D}";
            public static string CreateConference => ApiRoot;
            public static string DeleteConference(Guid hearingRefId, Guid conferenceId) => $"{ApiRoot}/{hearingRefId:D}/{conferenceId:D}";
            public static string GetConferencesForJudge(string username) => $"{ApiRoot}/today/judge?username={username}";
            public static string GetConferencesForVho => $"{ApiRoot}/today/vho";
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

            public static string GetHearingsByUsername(string username)
            {
                return $"{ApiRoot}/username/{username}";
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
        }
    }
}