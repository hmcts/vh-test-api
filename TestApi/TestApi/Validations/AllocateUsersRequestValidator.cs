﻿using System;
using FluentValidation;
using TestApi.Contract.Requests;
using TestApi.Domain.Enums;

namespace TestApi.Validations
{
    public class AllocateUsersRequestValidator : AbstractValidator<AllocateUsersRequest>
    {
        public const string EMPTY_USERS_ERROR_MESSAGE = "You must supply more than 1 usertype";
        public const string EMPTY_APPLICATION_ERROR_MESSAGE = "You must supply an application";
        public const string MISSING_ADMIN_USERS_ERROR_MESSAGE = "Users must include either case admin or video hearings officer";
        public const string MORE_THAN_ONE_JUDGE_ERROR_MESSAGE = "You can only specify 1 judge per allocation";
        public const string EXPIRES_IN_GREATER_THAN_ZERO_ERROR_MESSAGE = "Expires in must be greater than 0";
        public const string EXPIRES_IN_LESS_THAN_TWELVE_HOURS_ERROR_MESSAGE = "Expires in must be less than or equal to 12 hours";
        
        public AllocateUsersRequestValidator()
        {
            RuleFor(x => x.UserTypes)
                .NotEmpty().WithMessage(EMPTY_USERS_ERROR_MESSAGE);

            RuleFor(x => x.Application)
                .NotEmpty().WithMessage(EMPTY_APPLICATION_ERROR_MESSAGE);

            RuleFor(x => x.UserTypes)
                .Must(x => 
                    x.FindAll(x => x == UserType.CaseAdmin).Count + 
                    x.FindAll(x => (x == UserType.VideoHearingsOfficer)).Count > 0)
                .WithMessage(MISSING_ADMIN_USERS_ERROR_MESSAGE);

            RuleFor(x => x.UserTypes)
                .Must(x => x.FindAll(u => u.Equals(UserType.Judge)).Count <= 1)
                .WithMessage(MORE_THAN_ONE_JUDGE_ERROR_MESSAGE);

            RuleFor(x => x.ExpiryInMinutes)
                .GreaterThan(0).WithMessage(EXPIRES_IN_GREATER_THAN_ZERO_ERROR_MESSAGE);

            RuleFor(x => x.ExpiryInMinutes)
                .LessThanOrEqualTo(Convert.ToInt32(TimeSpan.FromHours(12).TotalMinutes)).WithMessage(EXPIRES_IN_LESS_THAN_TWELVE_HOURS_ERROR_MESSAGE);
        }
    }
}