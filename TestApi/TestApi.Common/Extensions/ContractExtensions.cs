using System;

namespace TestApi.Common.Extensions
{
    public static class ContractExtensions
    {
        public static Contract.Enums.Application MapToContractEnum(this Domain.Enums.Application application)
        {
            return Enum.Parse<Contract.Enums.Application>(application.ToString());
        }

        public static Contract.Enums.TestType MapToContractEnum(this Domain.Enums.TestType testType)
        {
            return Enum.Parse<Contract.Enums.TestType>(testType.ToString());
        }

        public static Contract.Enums.UserType MapToContractEnum(this Domain.Enums.UserType userType)
        {
            return Enum.Parse<Contract.Enums.UserType>(userType.ToString());
        }

        public static Domain.Enums.Application MapToContractEnum(this Contract.Enums.Application application)
        {
            return Enum.Parse<Domain.Enums.Application>(application.ToString());
        }

        public static Domain.Enums.TestType MapToContractEnum(this Contract.Enums.TestType testType)
        {
            return Enum.Parse<Domain.Enums.TestType>(testType.ToString());
        }

        public static Domain.Enums.UserType MapToContractEnum(this Contract.Enums.UserType userType)
        {
            return Enum.Parse<Domain.Enums.UserType>(userType.ToString());
        }
    }
}
