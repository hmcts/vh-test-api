using System;
using System.Collections.Generic;
using TestApi.Domain.Enums;

namespace TestApi.Domain.Helpers
{
    public class UserTypeName
    {
        public UserTypeName(UserType userType, string name)
        {
            UserType = userType;
            Name = name;
        }

        public UserType UserType { get; set; }
        public string Name { get; set; }

        public static UserTypeName Judge => new UserTypeName(UserType.Judge, "Judge");
        public static UserTypeName Individual => new UserTypeName(UserType.Individual, "Individual");
        public static UserTypeName Representative => new UserTypeName(UserType.Representative, "Representative");
        public static UserTypeName Observer => new UserTypeName(UserType.Observer, "Observer");
        public static UserTypeName PanelMember => new UserTypeName(UserType.PanelMember, "Panel Member");
        public static UserTypeName Winger => new UserTypeName(UserType.Winger, "Winger");
        public static UserTypeName CaseAdmin => new UserTypeName(UserType.CaseAdmin, "Case Admin");
        public static UserTypeName VideoHearingsOfficer =>
            new UserTypeName(UserType.VideoHearingsOfficer, "Video Hearings Officer");

        private static IEnumerable<UserTypeName> Values
        {
            get
            {
                yield return Judge;
                yield return Individual;
                yield return Representative;
                yield return Observer;
                yield return PanelMember;
                yield return Winger;
                yield return CaseAdmin;
                yield return VideoHearingsOfficer;
            }
        }

        public static string FromUserType(UserType userType)
        {
            foreach (var userTypeValues in Values)
                if (userType.Equals(userTypeValues.UserType))
                    return userTypeValues.Name;

            throw new ArgumentOutOfRangeException($"No user type found with user type '{userType}'");
        }

        public static UserType FromString(string userType)
        {
            foreach (var userTypeValues in Values)
                if (userType.Equals(userTypeValues.Name))
                    return userTypeValues.UserType;

            throw new ArgumentOutOfRangeException($"No user type found with name '{userType}'");
        }
    }
}