﻿using TestApi.Contract.Enums;

namespace TestApi.Contract.Requests
{
    /// <summary>
    ///     Creating a new user request
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        ///     Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     ContactEmail
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        ///     FirstName
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     LastName
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     DisplayName
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     User number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        ///     TestType
        /// </summary>
        public TestType TestType { get; set; }

        /// <summary>
        ///     UserType
        /// </summary>
        public UserType UserType { get; set; }

        /// <summary>
        ///     Application
        /// </summary>
        public Application Application { get; set; }

        /// <summary>
        ///     Is this a prod user
        /// </summary>
        public bool IsProdUser { get; set; }
    }
}