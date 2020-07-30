using System.Collections;

namespace TestApi.Contract.Responses
{
    /// <summary>Determine the health of the test api and connected apis</summary>
    public class HealthCheckResponse
    {
        /// <summary>The main response</summary>
        public HealthCheckResponse()
        {
            TestApiHealth = new HealthCheck();
            UserApiHealth = new HealthCheck();
            Version = new ApplicationVersion();
        }

        /// <summary>The health of the Test Api</summary>
        public HealthCheck TestApiHealth { get; set; }

        /// <summary>The health of the User Api</summary>
        public HealthCheck UserApiHealth { get; set; }

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
