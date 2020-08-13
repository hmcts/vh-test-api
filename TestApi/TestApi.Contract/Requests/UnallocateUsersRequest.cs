using System.Collections.Generic;

namespace TestApi.Contract.Requests
{
    /// <summary>
    ///     Unallocate a list of users
    /// </summary>
    public class UnallocateUsersRequest
    {
        /// <summary>
        ///     Usernames of the users to unallocate
        /// </summary>
        public List<string> Usernames { get; set; }
    }
}