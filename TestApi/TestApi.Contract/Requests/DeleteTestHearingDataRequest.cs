namespace TestApi.Contract.Requests
{
    /// <summary>
    /// Remove Hearing data request
    /// </summary>
    public class DeleteTestHearingDataRequest
    {
        /// <summary>
        /// Partial Hearing Case Name (must contain 'Test')
        /// </summary>
        public string PartialHearingCaseName { get; set; }

        /// <summary>
        /// Partial Hearing Case Number (must contain 'Test')
        /// </summary>
        public string PartialHearingCaseNumber { get; set; }

        /// <summary>
        /// The limit of how many hearings to search through for the title. Default is 1000
        /// </summary>
        public int? Limit { get; set; }
    }
}
