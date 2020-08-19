using System.Linq;
using TestApi.Domain.Enums;

namespace TestApi.Domain.Helpers
{
    public static class GetUserType
    {
        private const string UNDERSCORE = "_";

        public static UserType FromUserLastName(string text)
        {
            text = RemoveWhitespace(text);
            text = RemoveUnderscores(text);
            text = RemoveNumbers(text);
            text = AddSpacesBetweenCapitalLetters(text);
            return UserTypeName.FromString(text);
        }

        private static string RemoveWhitespace(string text)
        {
            return text.Trim();
        }

        private static string RemoveUnderscores(string text)
        {
            return text.Replace(UNDERSCORE, string.Empty).Trim();
        }

        private static string RemoveNumbers(string text)
        {
            return new string(text.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());
        }

        private static string AddSpacesBetweenCapitalLetters(string text)
        {
            return string.Concat(text.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }
    }
}