using System.Collections.Generic;
using TestApi.Domain.Enums;

namespace TestApi.Contract.Requests
{
    /// <summary>
    /// Allocate users request model
    /// </summary>
    public class AllocateUsersRequest
    {
        /// <summary>
        /// A list of the User types (e.g. Judge, Individual etc...)
        /// </summary>
        public List<UserType> UserTypes { get; set; }

        /// <summary>
        /// The Application to assign the users too (e.g. VideoWeb, AdminWeb etc...)
        /// </summary>
        public Application Application { get; set; }
    }
}
