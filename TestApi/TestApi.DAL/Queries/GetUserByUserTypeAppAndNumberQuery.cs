using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.Common.Extensions;
using TestApi.Common.Mappers;
using TestApi.Contract.Dtos;
using TestApi.DAL.Queries.Core;
using TestApi.Contract.Enums;

namespace TestApi.DAL.Queries
{
    public class GetUserByUserTypeAppAndNumberQuery : IQuery
    {
        public GetUserByUserTypeAppAndNumberQuery(UserType userType, Application application, int number, bool isProdUser)
        {
            UserType = userType;
            Application = application;
            Number = number;
            IsProdUser = isProdUser;
        }

        public UserType UserType { get; set; }
        public Application Application { get; set; }
        public int Number { get; set; }
        public bool IsProdUser { get; set; }
    }

    public class GetUserByUserTypeAppAndNumberQueryHandler : IQueryHandler<GetUserByUserTypeAppAndNumberQuery, UserDto>
    {
        private readonly TestApiDbContext _context;

        public GetUserByUserTypeAppAndNumberQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(GetUserByUserTypeAppAndNumberQuery query)
        {
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserType == query.UserType.MapToContractEnum() &&
                                           x.Application == query.Application.MapToContractEnum() &&
                                           x.Number == query.Number &&
                                           x.IsProdUser == query.IsProdUser);

            return user == null ? null : UserToUserDtoMapper.Map(user);
        }
    }
}