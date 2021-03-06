﻿using System;

namespace TestApi.Contract.Responses
{
    /// <summary>
    ///     Allocate a user request model
    /// </summary>
    public class AllocationDetailsResponse
    {
        /// <summary>Id</summary>
        public Guid Id { get; set; }

        /// <summary>User Id</summary>
        public Guid UserId { get; set; }

        /// <summary>Username</summary>
        public string Username { get; set; }

        /// <summary>ExpiresAt</summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>Allocated</summary>
        public bool Allocated { get; set; }

        /// <summary>Allocated By</summary>
        public string AllocatedBy { get; set; }
    }
}