using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.DAL.Queries
{
    public class GetUserByUserTypeApplicationAndNumberQuery : IQuery
    {
        public UserType UserType { get; set; }
        public Application Application { get; set; }
        public int Number { get; set; }


        public GetUserByUserTypeApplicationAndNumberQuery(UserType userType, Application application, int number)
        {
            UserType = userType;
            Application = application;
            Number = number;
        }
    }

    public class GetUserByUserTypeApplicationAndNumberQueryHandler : IQueryHandler<GetUserByUserTypeApplicationAndNumberQuery, User>
    {
        private readonly TestApiDbContext _context;

        public GetUserByUserTypeApplicationAndNumberQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByUserTypeApplicationAndNumberQuery query)
        {
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserType == query.UserType &&
                                           x.Application == query.Application &&
                                           x.Number == query.Number);

            if (user == null)
            {
                throw new UserNotFoundException(query.UserType, query.Application, query.Number);
            }

            return user;
        }
    }
}
