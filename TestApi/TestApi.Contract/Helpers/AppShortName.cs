using System;
using System.Collections.Generic;
using TestApi.Contract.Enums;

namespace TestApi.Contract.Helpers
{
    public class AppShortName
    {
        public AppShortName(Application application, string shortName)
        {
            Application = application;
            ShortName = shortName;
        }

        public Application Application { get; set; }
        public string ShortName { get; set; }

        public static AppShortName AdminWeb => new AppShortName(Application.AdminWeb, "AW");
        public static AppShortName BookingsApi => new AppShortName(Application.BookingsApi, "BA");
        public static AppShortName ServiceWeb => new AppShortName(Application.ServiceWeb, "SW");
        public static AppShortName TestApi => new AppShortName(Application.TestApi, "TA");
        public static AppShortName TestWeb => new AppShortName(Application.TestWeb, "TW");
        public static AppShortName UserApi => new AppShortName(Application.UserApi, "UA");
        public static AppShortName VideoApi => new AppShortName(Application.VideoApi, "VA");
        public static AppShortName VideoWeb => new AppShortName(Application.VideoWeb, "VW");
        public static AppShortName QueueSubscriber => new AppShortName(Application.QueueSubscriber, "BQS");

        private static IEnumerable<AppShortName> Values
        {
            get
            {
                yield return AdminWeb;
                yield return BookingsApi;
                yield return ServiceWeb;
                yield return TestApi;
                yield return TestWeb;
                yield return UserApi;
                yield return VideoApi;
                yield return VideoWeb;
                yield return QueueSubscriber;
            }
        }

        public static string FromApplication(Application application)
        {
            foreach (var app in Values)
                if (application.Equals(app.Application))
                    return app.ShortName;

            throw new ArgumentOutOfRangeException($"No application found with name '{application}'");
        }
    }
}