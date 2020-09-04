namespace TestApi.Contract.Responses
{
    /// <summary>Determine the health of the test api and connected apis</summary>
    public class HealthResponse
    {
        /// <summary>The main response</summary>
        public HealthResponse()
        {
            BookingsApiHealth = new HealthDetailsResponse();
            TestApiHealth = new HealthDetailsResponse();
            UserApiHealth = new HealthDetailsResponse();
            VideoApiHealth = new HealthDetailsResponse();
            Version = new AppVersionResponse();
        }

        /// <summary>The health of the Bookings Api</summary>
        public HealthDetailsResponse BookingsApiHealth { get; set; }

        /// <summary>The health of the Test Api</summary>
        public HealthDetailsResponse TestApiHealth { get; set; }

        /// <summary>The health of the User Api</summary>
        public HealthDetailsResponse UserApiHealth { get; set; }

        /// <summary>The health of the Video Api</summary>
        public HealthDetailsResponse VideoApiHealth { get; set; }

        /// <summary>Version of the Test Api</summary>
        public AppVersionResponse Version { get; set; }
    }
}