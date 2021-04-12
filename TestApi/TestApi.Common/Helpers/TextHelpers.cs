namespace TestApi.DAL.Helpers
{
    public static class TextHelpers
    {
        public static string ReplaceSpacesWithUnderscores(string text)
        {
            const string BLANK_SPACE = " ";
            const string UNDERSCORE = "_";
            return text.Replace(BLANK_SPACE, UNDERSCORE);
        }
    }
}
