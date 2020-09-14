using System.Collections.Generic;
using System.Linq;

namespace TestApi.Services.Helpers
{
    public static class ConvertGroupsStringToList
    {
        public static List<string> Convert(string text)
        {
            var groupsArrays = text.Split(",");
            return groupsArrays.Select(@group => @group.Trim()).ToList();
        }
    }
}
