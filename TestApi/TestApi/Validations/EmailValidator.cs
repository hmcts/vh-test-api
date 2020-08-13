using System;
using System.Net.Mail;

namespace TestApi.Validations
{
    public static class EmailValidator
    {
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var address = new MailAddress(email);
                return address.Address.Length > 0;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}