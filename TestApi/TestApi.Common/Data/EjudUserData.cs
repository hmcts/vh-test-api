using TestApi.DAL.Helpers;

namespace TestApi.Common.Data
{
    public static class EjudUserData
    {
        public const string AUTOMATED_FIRST_NAME_PREFIX = "Auto";
        public const string MANUAL_FIRST_NAME_PREFIX = "Manual";
        public static string LAST_NAME(int number) => $"Judge {number}";
        public static string DISPLAY_NAME(string firstName, string lastName) => $"{firstName} {lastName}";
        public static string USERNAME(string firstName, string lastName, string domain) => $"{firstName}_{TextHelpers.ReplaceSpacesWithUnderscores(lastName)}@{domain}";
        public static string CONTACT_EMAIL(string firstName, string lastName, string domain) => $"{firstName}_{TextHelpers.ReplaceSpacesWithUnderscores(lastName)}@{domain}";
    }
}
