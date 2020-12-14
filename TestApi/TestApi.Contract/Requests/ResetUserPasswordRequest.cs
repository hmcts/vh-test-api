namespace TestApi.Contract.Requests
{
    /// <summary>
    /// Reset user password request
    /// </summary>
    public class ResetUserPasswordRequest
    {
        /// <summary>
        /// Username of user to reset
        /// </summary>
        public string Username { get; set; }
    }
}
