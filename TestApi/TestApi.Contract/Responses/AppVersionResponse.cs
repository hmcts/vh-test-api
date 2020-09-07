namespace TestApi.Contract.Responses
{
    /// <summary>Version of the app</summary>
    public class AppVersionResponse
    {
        /// <summary>Version of the app</summary>
        public string Version { get; set; }

        /// <summary>File Version of the app</summary>
        public string FileVersion { get; set; }

        /// <summary>Information Version of the app</summary>
        public string InformationVersion { get; set; }
    }
}
