using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.Common.Extensions;
using TestApi.DAL.Helpers;
using TestApi.DAL.Queries.Core;
using TestApi.Contract.Enums;
using TestApi.Domain;

namespace TestApi.DAL.Queries
{
    public class GetNextUserNumberByUserTypeQuery : IQuery
    {
        public GetNextUserNumberByUserTypeQuery(UserType userType, Application application, bool isProdUser, TestType testType)
        {
            UserType = userType;
            Application = application;
            IsProdUser = isProdUser;
            TestType = testType;
        }

        public UserType UserType { get; set; }
        public Application Application { get; set; }
        public bool IsProdUser { get; set; }
        public TestType TestType { get; set; }
    }

    public class GetNextUserNumberByUserTypeQueryHandler : IQueryHandler<GetNextUserNumberByUserTypeQuery, Integer>
    {
        private readonly TestApiDbContext _context;

        public GetNextUserNumberByUserTypeQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<Integer> Handle(GetNextUserNumberByUserTypeQuery query)
        {
            List<User> users;

            if (query.TestType == TestType.Automated)
            {
                users = await _context.Users
                    .Where(x => x.UserType == query.UserType.MapToContractEnum() && x.Application == query.Application.MapToContractEnum() && x.IsProdUser == query.IsProdUser)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                users = await _context.Users
                    .Where(x => x.UserType == query.UserType.MapToContractEnum() && x.IsProdUser == query.IsProdUser)
                    .AsNoTracking()
                    .ToListAsync();
            }
            
            if (users.Count.Equals(0)) return 1;

            return users.Select(user => user.Number).AsEnumerable().Max() + 1;
        }
    }
}