using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.Common.Extensions;
using TestApi.Common.Mappers;
using TestApi.Contract.Dtos;
using TestApi.DAL.Queries.Core;
using TestApi.Contract.Enums;

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

    public class GetAllUsersByFilterQueryHandler : IQueryHandler<GetAllUsersByFilterQuery, List<UserDto>>
    {
        private readonly TestApiDbContext _context;

        public GetAllUsersByFilterQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> Handle(GetAllUsersByFilterQuery query)
        {
            var  users = await _context.Users
                .Where(x => 
                    x.UserType == query.UserType.MapToContractEnum() && 
                    x.TestType == query.TestType.MapToContractEnum() &&
                    x.Application == query.Application.MapToContractEnum() && 
                    x.IsProdUser == query.IsProdUser)
                .AsNoTracking()
                .ToListAsync();

            return users.Select(UserToUserDtoMapper.Map).ToList();
        }
    }
}