using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.DAL.Queries
{
    public class GetAllUsersByFilterQuery : IQuery
    {
        public GetAllUsersByFilterQuery(UserType userType, TestType testType, Application application, bool isProdUser)
        {
            UserType = userType;
            TestType = testType;
            Application = application;
            IsProdUser = isProdUser;
        }

        public UserType UserType { get; set; }
        public TestType TestType { get; set; }
        public Application Application { get; set; }
        public bool IsProdUser { get; set; }
    }

    public class GetAllUsersByFilterQueryHandler : IQueryHandler<GetAllUsersByFilterQuery, List<User>>
    {
        private readonly TestApiDbContext _context;

        public GetAllUsersByFilterQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Handle(GetAllUsersByFilterQuery query)
        {
            return await _context.Users
                .Where(x => 
                    x.UserType == query.UserType && 
                    x.TestType == query.TestType &&
                    x.Application == query.Application && 
                    x.IsProdUser == query.IsProdUser)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}