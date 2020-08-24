namespace TestApi.Common.Data
{
    public static class DefaultData
    {
        public static string CREATED_BY(string usernameStem) => $"automation_test@{usernameStem}";
        public static bool AUDIO_RECORDING_REQUIRED = false;
        public static string CANCELLATION_REASON = "Cancellation reason";
        public const string CASE_NAME_PREFIX = "Automated Test";
        public const string CASE_TYPE_NAME = "Civil Money Claims";
        public const string HEARING_ROOM_NAME = "Room 1";
        public const string HEARING_TYPE_NAME = "Application to Set Judgment Aside";
        public const bool IS_LEAD_CASE = false;
        public static readonly string MIDDLE_NAME = string.Empty;
        public const string OTHER_INFORMATION = "Other information";
        public static bool QUESTIONNAIRE_NOT_REQUIRED = true;
        public const string REFERENCE = "Reference";
        public const int SCHEDULED_DURATION = 60;
        public const string TELEPHONE_NUMBER = "+44(0)71234567891";
        public const string TITLE = "Mrs";
        public const string UPDATED_TEXT = "UPDATED";
        public const string VENUE_NAME = "Birmingham Civil and Family Justice Centre";
        public const string VIDEO_EVENT_REASON = "Automated test";
        public const string ALTERNATIVE_VENUE_NAME = "Manchester Civil and Family Justice Centre";
        public const string FIRST_CASE_ROLE_NAME = "Claimant";
        public const string SECOND_CASE_ROLE_NAME = "Defendant";
        public const string FIRST_INDV_HEARING_ROLE_NAME = "Claimant LIP";
        public const string SECOND_INDV_HEARING_ROLE_NAME = "Defendant LIP";
        public const string REPRESENTATIVE_HEARING_ROLE_NAME = "Representative";
        public const string MEETING_ROOM_ADMIN_URL = "url";
        public const string MEETING_ROOM_JUDGE_URL = "url";
        public const string MEETING_ROOM_PARTICIPANT_URL = "url";
        public const string MEETING_ROOM_PEXIP_NODE = "NODE";
        public const string MEETING_ROOM_PEXIP_SELF_TEST_NODE = "NODE";
        public const string FAKE_JUDGE_GROUP_1 = "Judge group 1";
        public const string FAKE_JUDGE_GROUP_2 = "Judge group 2";
        public const string FAKE_INDIVIDUAL_GROUP_1 = "Individual group 1";
        public const string FAKE_INDIVIDUAL_GROUP_2 = "Individual group 2";
        public const string FAKE_REPRESENTATIVE_GROUP_1 = "Representative group 1";
        public const string FAKE_REPRESENTATIVE_GROUP_2 = "Representative group 2";
        public const string FAKE_CASE_ADMIN_GROUP_1 = "Case Admin group 1";
        public const string FAKE_CASE_ADMIN_GROUP_2 = "Case Admin group 2";
        public const string FAKE_VIDEO_HEARINGS_OFFICER_GROUP_1 = "Video Hearings Officer group 1";
        public const string FAKE_VIDEO_HEARINGS_OFFICER_GROUP_2 = "Video Hearings Officer group 2";
        public const string FAKE_PEXIP_GROUP_1 = "Pexip group 1";
        public const string FAKE_PEXIP_GROUP_2 = "Pexip group 2";
        public const string FAKE_TEST_GROUP= "Test group 1";
        public const string FAKE_PERFORMANCE_TEST_GROUP = "Performance test group 1";
        public const string EXISTING_CONTACT_EMAIL = "made_up_username@email.com";
        public const string NON_EXISTENT_CONTACT_EMAIL = "made_up_email@email.com";
        public const string NON_EXISTENT_USERNAME = "made_up_username@email.com";
        public const string FAKE_EMAIL_STEM = "made_up_email_stem_for_test";
    }
}
