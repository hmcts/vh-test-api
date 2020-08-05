using System.Collections;

namespace TestApi.Contract.Responses
{
    /// <summary>Determine the health of the test api and connected apis</summary>
    public class HealthCheckResponse
    {
        /// <summary>The main response</summary>
        public HealthCheckResponse()
        {
            BookingsApiHealth = new HealthCheck();
            TestApiHealth = new HealthCheck();
            UserApiHealth = new HealthCheck();
            VideoApiHealth = new HealthCheck();
            Version = new ApplicationVersion();
        }

        /// <summary>The health of the Bookings Api</summary>
        public HealthCheck BookingsApiHealth { get; set; }

        /// <summary>The health of the Test Api</summary>
        public HealthCheck TestApiHealth { get; set; }

        /// <summary>The health of the User Api</summary>
        public HealthCheck UserApiHealth { get; set; }

        /// <summary>The health of the Video Api</summary>
        public HealthCheck VideoApiHealth { get; set; }

        /// <summary>Version of the Test Api</summary>
        public ApplicationVersion Version { get; set; }

        /// <summary>Version of the app</summary>
        public class ApplicationVersion
        {
            /// <summary>Version of the app</summary>
            public string Version { get; set; }
        }

        /// <summary>Healthcheck response</summary>
        public class HealthCheck
        {
            /// <summary>Whether the check was successful</summary>
            public bool Successful { get; set; }

            /// <summary>Any associated error message with an unsuccessful response</summary>
            public string ErrorMessage { get; set; }

            /// <summary>Any associated data</summary>
            public IDictionary Data { get; set; }
        }
    }
}
