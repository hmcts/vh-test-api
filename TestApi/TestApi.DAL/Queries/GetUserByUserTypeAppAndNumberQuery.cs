#nullable enable
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.DAL.Queries
{
    public class GetUserByUserTypeAppAndNumberQuery : IQuery
    {
        public UserType UserType { get; set; }
        public Application Application { get; set; }
        public int Number { get; set; }


        public GetUserByUserTypeAppAndNumberQuery(UserType userType, Application application, int number)
        {
            UserType = userType;
            Application = application;
            Number = number;
        }
    }

    public class GetUserByUserTypeAppAndNumberQueryHandler : IQueryHandler<GetUserByUserTypeAppAndNumberQuery, User>
    {
        private readonly TestApiDbContext _context;

        public GetUserByUserTypeAppAndNumberQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByUserTypeAppAndNumberQuery query)
        {
            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserType == query.UserType &&
                                           x.Application == query.Application &&
                                           x.Number == query.Number);
        }
    }
}
