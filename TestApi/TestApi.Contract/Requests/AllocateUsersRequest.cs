using System.Collections.Generic;
using TestApi.Contract.Enums;

namespace TestApi.Contract.Requests
{
    /// <summary>Allocate users request model</summary>
    public class AllocateUsersRequest
    {
        /// <summary>The Application to assign the users too (e.g. VideoWeb, AdminWeb etc...)</summary>
        public Application Application { get; set; }

        /// <summary>Allocate a different expiry time for the user other than the default 10 minutes</summary>
        public int ExpiryInMinutes { get; set; }

        /// <summary>Will the user be required for a prod test?</summary>
        public bool IsProdUser { get; set; }

        /// <summary>The type of test. Default is Automated</summary>
        public TestType TestType { get; set; }

        /// <summary>A list of the User types (e.g. Judge, Individual etc...)</summary>
        public List<UserType> UserTypes { get; set; }

        /// <summary>The user that allocated the user</summary>
        public string AllocatedBy { get; set; }

        /// <summary>Should the Judge, Panel Member or Winger users come from the eJud AAD?</summary>
        public bool IsEjud { get; set; }
    }
}