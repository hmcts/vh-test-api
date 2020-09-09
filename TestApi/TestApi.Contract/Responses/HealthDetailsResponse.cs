using System.Collections;

namespace TestApi.Contract.Responses
{
    /// <summary>Healthcheck response</summary>
    public class HealthDetailsResponse
    {
        /// <summary>Whether the check was successful</summary>
        public bool Successful { get; set; }

        /// <summary>Any associated error message with an unsuccessful response</summary>
        public string ErrorMessage { get; set; }

        /// <summary>Any associated data</summary>
        public IDictionary Data { get; set; }
    }
}
