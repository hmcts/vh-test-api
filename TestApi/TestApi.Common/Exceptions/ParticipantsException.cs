using System;
using System.Collections.Generic;

namespace TestApi.Common.Exceptions
{
    public class ParticipantsException : Exception
    {
        public ParticipantsException(List<string> usernames) : base($"No case admin user found in users: {usernames}")
        {
        }
    }
}