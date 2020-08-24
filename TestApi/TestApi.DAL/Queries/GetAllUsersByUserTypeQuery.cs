using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.DAL.Queries
{
    public class GetAllUsersByUserTypeQuery : IQuery
    {
        public GetAllUsersByUserTypeQuery(UserType userType, Application application, bool isProdUser)
        {
            UserType = userType;
            Application = application;
            IsProdUser = isProdUser;
        }

        public UserType UserType { get; set; }
        public Application Application { get; set; }
        public bool IsProdUser { get; set; }
    }

    public class GetAllUsersByUserTypeQueryHandler : IQueryHandler<GetAllUsersByUserTypeQuery, List<User>>
    {
        private readonly TestApiDbContext _context;

        public GetAllUsersByUserTypeQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Handle(GetAllUsersByUserTypeQuery query)
        {
            return await _context.Users
                .Where(x => x.UserType == query.UserType && x.Application == query.Application && x.IsProdUser == query.IsProdUser)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}