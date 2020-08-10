using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Contract.Responses;
using TestApi.Domain.Enums;
using TestApi.Mappings;

namespace TestApi.UnitTests.Mappings
{
    public class UserDetailsToUserMapperTests
    {
        [Test]
        public void Should_map_all_properties() 
        {
            var user = new UserBuilder("made_up_email_stem_for_test", 1)
                .WithUserType(UserType.Individual)
                .ForApplication(Application.TestApi)
                .BuildUser();

            var userDetails = new List<UserDetailsResponse>
            {
                new UserDetailsResponse()
                {
                    Application = user.Application,
                    ContactEmail = user.ContactEmail,
                    CreatedDate = DateTime.UtcNow,
                    DisplayName = user.DisplayName,
                    FirstName = user.FirstName,
                    Id = Guid.NewGuid(),
                    LastName = user.LastName,
                    Number = user.Number,
                    Username = user.Username,
                    UserType = user.UserType
                }
            };

            var response = UserDetailsResponseToUserMapper.Map(userDetails);
            response.First().Should().BeEquivalentTo(user, options => options.Excluding(x => x.CreatedDate));
        }
    }
}
