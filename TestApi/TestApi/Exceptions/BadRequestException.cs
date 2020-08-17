using System;

namespace TestApi.Exceptions
{
    /// <summary>
    ///     Exception to throw when input data passed downstream from the api input is in an invalid format
    /// </summary>
    [Serializable]
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}